import { Component, OnInit, signal, computed, inject } from '@angular/core';
import { ActivatedRoute, RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HotelService, Hotel, Room, RoomAvailabilityResponse } from '../../services/hotel.service';
import { BookingModalComponent } from '../booking-modal/booking-modal.component';

@Component({
  selector: 'app-hotel-detail',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule, BookingModalComponent],
  templateUrl: './hotel-detail.component.html',
  styleUrls: ['./hotel-detail.component.scss']
})
export class HotelDetailComponent implements OnInit {
  private route = inject(ActivatedRoute);
  private hotelService = inject(HotelService);
  
  Math = Math;
  
  // Data signals
  hotel = signal<Hotel | null>(null);
  allRooms = signal<Room[]>([]);
  loading = signal(true);
  error = signal<string | null>(null);
  
  // Filter signals
  searchTerm = signal('');
  typeFilter = signal('');
  availableOnly = signal(false);
  
  // Booking modal state
  isBookingModalOpen = signal(false);
  selectedRoom = signal<Room | null>(null);
  
  // Date selection for availability
  checkInDate = signal<Date | null>(null);
  checkOutDate = signal<Date | null>(null);
  availabilityChecked = signal(false);
  availabilityMap = signal<Map<number, boolean>>(new Map()); // Maps roomId to availability status
  checkingAvailability = signal(false);
  
  // Get unique room types
  roomTypes = computed(() => {
    const rooms = this.allRooms();
    const types = new Set<string>();
    rooms.forEach(room => {
      if (room.type) types.add(room.type);
    });
    return Array.from(types).sort();
  });
  
  // Filtered rooms with availability check
  filteredRooms = computed(() => {
    const rooms = this.allRooms();
    const search = this.searchTerm().toLowerCase();
    const type = this.typeFilter();
    const onlyAvailable = this.availableOnly();
    const availabilityMap = this.availabilityMap();
    const hasCheckedAvailability = this.availabilityChecked();
    const checkIn = this.checkInDate();
    const checkOut = this.checkOutDate();
    
    // First, map rooms with availability info
    const roomsWithAvailability = rooms.map(room => {
      let isAvailableForDates = true;
      
      // If availability has been checked and we have dates, use the map
      if (hasCheckedAvailability && checkIn && checkOut && availabilityMap.has(room.id)) {
        isAvailableForDates = availabilityMap.get(room.id) ?? true;
      }
      
      const matchesSearch = search === '' || 
        room.type.toLowerCase().includes(search) ||
        (room.description?.toLowerCase().includes(search) || false);
      
      const matchesType = type === '' || room.type === type;
      const matchesAvailability = !onlyAvailable || isAvailableForDates;
      
      return {
        ...room,
        isAvailable: isAvailableForDates,
        matchesSearch,
        matchesType,
        matchesAvailability
      };
    });
    
    // Then filter
    return roomsWithAvailability.filter(room => 
      room.matchesSearch && room.matchesType && room.matchesAvailability
    );
  });
  
  // Room stats based on filtered results
  roomStats = computed(() => {
    const total = this.allRooms().length;
    const filtered = this.filteredRooms();
    const availableCount = filtered.filter(r => r.isAvailable).length;
    return { total, available: availableCount, booked: total - availableCount };
  });

  ngOnInit(): void {
    const id = Number(this.route.snapshot.paramMap.get('id'));
    this.loadHotel(id);
    this.loadRooms(id);
  }

  loadHotel(id: number): void {
    this.hotelService.getHotelById(id).subscribe({
      next: (data) => {
        this.hotel.set(data);
      },
      error: (err) => {
        console.error('Error loading hotel:', err);
        this.error.set('Failed to load hotel details');
      }
    });
  }

  loadRooms(hotelId: number): void {
    this.loading.set(true);
    this.hotelService.getRoomsByHotel(hotelId).subscribe({
      next: (data) => {
        this.allRooms.set(data);
        this.loading.set(false);
      },
      error: (err) => {
        console.error('Error loading rooms:', err);
        this.error.set('Failed to load rooms');
        this.loading.set(false);
      }
    });
  }

  onSearch(event: Event): void {
    const value = (event.target as HTMLInputElement).value;
    this.searchTerm.set(value);
  }

  onFilterByType(event: Event): void {
    const value = (event.target as HTMLSelectElement).value;
    this.typeFilter.set(value);
  }
  
  onAvailableOnlyChange(event: Event): void {
    const checked = (event.target as HTMLInputElement).checked;
    this.availableOnly.set(checked);
  }

  clearFilters(): void {
    this.searchTerm.set('');
    this.typeFilter.set('');
    this.availableOnly.set(false);
  }
  
  // Date selection methods
  onCheckInDateChange(event: Event): void {
    const input = event.target as HTMLInputElement;
    const value = input.value;
    if (value) {
      this.checkInDate.set(new Date(value));
      // Reset availability when dates change
      this.availabilityChecked.set(false);
      this.availabilityMap.set(new Map());
    } else {
      this.checkInDate.set(null);
    }
  }
  
  onCheckOutDateChange(event: Event): void {
    const input = event.target as HTMLInputElement;
    const value = input.value;
    if (value) {
      this.checkOutDate.set(new Date(value));
      this.availabilityChecked.set(false);
      this.availabilityMap.set(new Map());
    } else {
      this.checkOutDate.set(null);
    }
  }
  
  checkAvailability(): void {
    const checkIn = this.checkInDate();
    const checkOut = this.checkOutDate();
    const hotelId = this.hotel()?.id;
    
    if (!checkIn || !checkOut) {
      alert('Please select both check-in and check-out dates');
      return;
    }
    
    if (checkIn >= checkOut) {
      alert('Check-out date must be after check-in date');
      return;
    }
    
    if (!hotelId) {
      alert('Hotel information not available');
      return;
    }
    
    this.checkingAvailability.set(true);
    
    // Call API to check availability for all rooms in this hotel
    this.hotelService.checkRoomsAvailability(hotelId, checkIn, checkOut).subscribe({
      next: (responses) => {
        const newAvailabilityMap = new Map<number, boolean>();
        responses.forEach(response => {
          newAvailabilityMap.set(response.roomId, response.isAvailable);
        });
        this.availabilityMap.set(newAvailabilityMap);
        this.availabilityChecked.set(true);
        this.checkingAvailability.set(false);
      },
      error: (err) => {
        console.error('Error checking availability:', err);
        alert('Failed to check room availability. Please try again.');
        this.checkingAvailability.set(false);
      }
    });
  }

  openBookingModal(room: Room): void {
    this.selectedRoom.set(room);
    this.isBookingModalOpen.set(true);
  }
  
  closeBookingModal(): void {
    this.isBookingModalOpen.set(false);
    this.selectedRoom.set(null);
  }
  
  onBookingComplete(): void {
    // Refresh rooms after booking
    const hotelId = Number(this.route.snapshot.paramMap.get('id'));
    this.loadRooms(hotelId);
    // Reset availability state
    this.availabilityChecked.set(false);
    this.availabilityMap.set(new Map());
    this.checkInDate.set(null);
    this.checkOutDate.set(null);
  }
}