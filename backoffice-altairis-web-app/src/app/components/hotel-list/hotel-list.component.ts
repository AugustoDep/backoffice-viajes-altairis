import { Component, OnInit, signal, computed, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { HotelService, Hotel } from '../../services/hotel.service';
import { HotelFormModalComponent } from '../hotel-form-modal/hotel-form-modal.component';

@Component({
  selector: 'app-hotel-list',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule, HotelFormModalComponent],
  templateUrl: './hotel-list.component.html',
  styleUrls: ['./hotel-list.component.scss']
})
export class HotelListComponent implements OnInit {
  private hotelService = inject(HotelService);
  
  // Make Math available in template
  Math = Math;
  
  // Pagination settings
  pageSize = 25;
  currentPage = signal(1);
  
  // Data signals
  hotels = signal<Hotel[]>([]);
  totalCount = signal(0);
  totalPages = signal(0);
  loading = signal(true);
  error = signal<string | null>(null);
  
  // Modal state
  isModalOpen = signal(false);
  modalMode: 'create' | 'edit' = 'create';
  selectedHotel: Hotel | null = null;
  
  // Filter signals
  searchTerm = signal('');
  starFilter = signal('');
  
  // Paginated hotels - directly from API response
  paginatedHotels = computed(() => this.hotels());
  
  // Page numbers array for pagination
  pageNumbers = computed(() => {
    const total = this.totalPages();
    const current = this.currentPage();
    const pages: (number | string)[] = [];
    
    if (total <= 7) {
      for (let i = 1; i <= total; i++) {
        pages.push(i);
      }
    } else {
      if (current <= 3) {
        pages.push(1, 2, 3, 4, '...', total);
      } else if (current >= total - 2) {
        pages.push(1, '...', total - 3, total - 2, total - 1, total);
      } else {
        pages.push(1, '...', current - 1, current, current + 1, '...', total);
      }
    }
    
    return pages;
  });
  
  // Display range
  displayRange = computed(() => {
    const start = (this.currentPage() - 1) * this.pageSize + 1;
    const end = Math.min(this.currentPage() * this.pageSize, this.totalCount());
    return { start, end };
  });

  ngOnInit(): void {
    this.loadHotels();
  }

  loadHotels(): void {
    this.loading.set(true);
    this.error.set(null);
    
    const starValue = this.starFilter() ? parseInt(this.starFilter()) : undefined;
    
    this.hotelService.getPaginatedHotels(
      this.currentPage(),
      this.pageSize,
      this.searchTerm(),
      starValue
    ).subscribe({
      next: (response) => {
        this.hotels.set(response.hotels);
        this.totalCount.set(response.totalCount);
        this.totalPages.set(response.totalPages);
        this.loading.set(false);
      },
      error: (err) => {
        console.error('Error loading hotels:', err);
        this.error.set('Failed to load hotels. Make sure the backend is running.');
        this.loading.set(false);
      }
    });
  }

  // Modal methods
  openCreateModal(): void {
    this.modalMode = 'create';
    this.selectedHotel = null;
    this.isModalOpen.set(true);
  }
  
  openEditModal(hotel: Hotel): void {
    this.modalMode = 'edit';
    this.selectedHotel = hotel;
    this.isModalOpen.set(true);
  }
  
  onModalClose(): void {
    this.isModalOpen.set(false);
    this.selectedHotel = null;
  }
  
  onHotelSaved(savedHotel: Hotel): void {
    // Reload the current page to reflect changes
    this.loadHotels();
  }

  onSearch(event: Event): void {
    const value = (event.target as HTMLInputElement).value;
    this.searchTerm.set(value);
    this.currentPage.set(1);
    this.loadHotels();
  }

  onFilterByStars(event: Event): void {
    const value = (event.target as HTMLSelectElement).value;
    this.starFilter.set(value);
    this.currentPage.set(1);
    this.loadHotels();
  }

  goToPage(page: number | string): void {
    if (typeof page === 'number') {
      this.currentPage.set(page);
      this.loadHotels();
      document.querySelector('.table-wrapper')?.scrollIntoView({ behavior: 'smooth' });
    }
  }

  previousPage(): void {
    if (this.currentPage() > 1) {
      this.currentPage.set(this.currentPage() - 1);
      this.loadHotels();
      document.querySelector('.table-wrapper')?.scrollIntoView({ behavior: 'smooth' });
    }
  }

  nextPage(): void {
    if (this.currentPage() < this.totalPages()) {
      this.currentPage.set(this.currentPage() + 1);
      this.loadHotels();
      document.querySelector('.table-wrapper')?.scrollIntoView({ behavior: 'smooth' });
    }
  }

  deleteHotel(id: number): void {
    if (confirm('Are you sure you want to delete this hotel? This action cannot be undone.')) {
      this.hotelService.deleteHotel(id).subscribe({
        next: () => {
          // Reload current page after deletion
          this.loadHotels();
        },
        error: (err) => {
          console.error('Error deleting hotel:', err);
          alert('Failed to delete hotel');
        }
      });
    }
  }

  refresh(): void {
    this.currentPage.set(1);
    this.searchTerm.set('');
    this.starFilter.set('');
    this.loadHotels();
  }

  exportToCSV(): void {
    // For CSV export, export current page only
    const headers = ['ID', 'Name', 'City', 'Country', 'Stars', 'Rooms', 'Address'];
    const rows = this.hotels().map(hotel => [
      hotel.id,
      hotel.name,
      hotel.city,
      hotel.country,
      hotel.stars,
      hotel.rooms?.length || 0,
      hotel.address
    ]);
    
    const csvContent = [
      headers.join(','),
      ...rows.map(row => row.map(cell => `"${cell}"`).join(','))
    ].join('\n');
    
    const blob = new Blob([csvContent], { type: 'text/csv;charset=utf-8;' });
    const link = document.createElement('a');
    const url = URL.createObjectURL(blob);
    link.setAttribute('href', url);
    link.setAttribute('download', `hotels_page_${this.currentPage()}.csv`);
    link.style.visibility = 'hidden';
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
  }
}