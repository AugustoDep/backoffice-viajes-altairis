import { Component, EventEmitter, Input, Output, signal, computed, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HotelService, Hotel, CreateHotelDto } from '../../services/hotel.service';

@Component({
  selector: 'app-hotel-form-modal',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './hotel-form-modal.component.html',
  styleUrls: ['./hotel-form-modal.component.scss']
})
export class HotelFormModalComponent {
  private hotelService = inject(HotelService);
  
  @Input() isOpen = false;
  @Input() mode: 'create' | 'edit' = 'create';
  @Input() hotel: Hotel | null = null;
  
  @Output() close = new EventEmitter<void>();
  @Output() saved = new EventEmitter<Hotel>();
  
  // Form data
  formData = signal({
    name: '',
    description: '',
    country: '',
    city: '',
    address: '',
    mainPhoto: '',
    stars: 3
  });
  
  loading = signal(false);
  error = signal<string | null>(null);
  
  // Available star ratings
  starRatings = [1, 2, 3, 4, 5];
  
  // Computed title based on mode
  title = computed(() => this.mode === 'create' ? 'Add New Hotel' : 'Edit Hotel');
  submitButtonText = computed(() => this.mode === 'create' ? 'Create Hotel' : 'Save Changes');
  
  constructor() {
    // Watch for hotel changes to populate form in edit mode
    this.populateForm();
  }
  
  populateForm(): void {
    if (this.mode === 'edit' && this.hotel) {
      this.formData.set({
        name: this.hotel.name || '',
        description: this.hotel.description || '',
        country: this.hotel.country || '',
        city: this.hotel.city || '',
        address: this.hotel.address || '',
        mainPhoto: this.hotel.mainPhoto || '',
        stars: this.hotel.stars || 3
      });
    } else {
      // Reset form for create mode
      this.resetForm();
    }
  }
  
  resetForm(): void {
    this.formData.set({
      name: '',
      description: '',
      country: '',
      city: '',
      address: '',
      mainPhoto: '',
      stars: 3
    });
    this.error.set(null);
  }
  
  onClose(): void {
    this.resetForm();
    this.close.emit();
  }
  
  onSubmit(): void {
    const data = this.formData();
    
    // Validation
    if (!data.name.trim()) {
      this.error.set('Hotel name is required');
      return;
    }
    if (!data.country.trim()) {
      this.error.set('Country is required');
      return;
    }
    if (!data.city.trim()) {
      this.error.set('City is required');
      return;
    }
    if (!data.address.trim()) {
      this.error.set('Address is required');
      return;
    }
    
    this.loading.set(true);
    this.error.set(null);
    
    if (this.mode === 'create') {
      this.hotelService.createHotel(data).subscribe({
        next: (newHotel) => {
          this.loading.set(false);
          this.saved.emit(newHotel);
          this.onClose();
        },
        error: (err) => {
          this.loading.set(false);
          this.error.set('Failed to create hotel. Please try again.');
          console.error('Error creating hotel:', err);
        }
      });
    } else if (this.mode === 'edit' && this.hotel) {
      const updateData = {
        id: this.hotel.id,
        ...data
      };
      this.hotelService.updateHotel(this.hotel.id, updateData).subscribe({
        next: () => {
          this.loading.set(false);
          // Emit updated hotel with new data
          this.saved.emit({ ...this.hotel, ...data } as Hotel);
          this.onClose();
        },
        error: (err) => {
          this.loading.set(false);
          this.error.set('Failed to update hotel. Please try again.');
          console.error('Error updating hotel:', err);
        }
      });
    }
  }
  
  updateStars(stars: number): void {
    this.formData.update(data => ({ ...data, stars }));
  }
}