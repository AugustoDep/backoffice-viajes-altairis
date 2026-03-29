import { Component, EventEmitter, Input, Output, signal, computed, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HotelService, Room, CreateBookingDto } from '../../services/hotel.service';

@Component({
  selector: 'app-booking-modal',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './booking-modal.component.html',
  styleUrls: ['./booking-modal.component.scss']
})
export class BookingModalComponent {
  private hotelService = inject(HotelService);
  
  Math = Math;
  
  @Input() isOpen = false;
  @Input() room: Room | null = null;
  @Input() hotelName: string = '';
  
  @Output() close = new EventEmitter<void>();
  @Output() booked = new EventEmitter<void>();
  
  // Form data
  formData = {
    customerName: '',
    customerEmail: '',
    customerPhone: '',
    checkInDate: '',
    checkOutDate: '',
    adults: 1,
    children: 0
  };
  
  // Payment method
  paymentMethod: 'credit_card' | 'paypal' | 'mock' = 'mock';
  cardDetails = {
    cardNumber: '',
    cardName: '',
    expiryDate: '',
    cvv: ''
  };
  
  loading = signal(false);
  error = signal<string | null>(null);
  step = signal(1); // 1: Booking details, 2: Payment, 3: Confirmation
  
  // Computed values
  nights = computed(() => {
    if (!this.formData.checkInDate || !this.formData.checkOutDate) return 1;
    const checkIn = new Date(this.formData.checkInDate);
    const checkOut = new Date(this.formData.checkOutDate);
    const diffTime = Math.abs(checkOut.getTime() - checkIn.getTime());
    return Math.ceil(diffTime / (1000 * 60 * 60 * 24)) || 1;
  });
  
  subtotal = computed(() => {
    if (!this.room) return 0;
    return this.room.pricePerNight * this.nights();
  });
  
  tax = computed(() => {
    return this.subtotal() * 0.18; // 18% tax
  });
  
  total = computed(() => {
    return this.subtotal() + this.tax();
  });
  
  // Available dates (today onwards)
  minDate = new Date().toISOString().split('T')[0];
  
  onClose(): void {
    this.resetForm();
    this.close.emit();
  }
  
  resetForm(): void {
    this.formData = {
      customerName: '',
      customerEmail: '',
      customerPhone: '',
      checkInDate: '',
      checkOutDate: '',
      adults: 1,
      children: 0
    };
    this.cardDetails = {
      cardNumber: '',
      cardName: '',
      expiryDate: '',
      cvv: ''
    };
    this.error.set(null);
    this.step.set(1);
  }
  
  nextStep(): void {
    if (this.step() === 1) {
      // Validate booking details
      if (!this.formData.customerName.trim()) {
        this.error.set('Customer name is required');
        return;
      }
      if (!this.formData.customerEmail.trim() || !this.isValidEmail(this.formData.customerEmail)) {
        this.error.set('Valid email is required');
        return;
      }
      if (!this.formData.checkInDate || !this.formData.checkOutDate) {
        this.error.set('Check-in and check-out dates are required');
        return;
      }
      if (new Date(this.formData.checkInDate) >= new Date(this.formData.checkOutDate)) {
        this.error.set('Check-out date must be after check-in date');
        return;
      }
      
      this.error.set(null);
      this.step.set(2);
    } else if (this.step() === 2) {
      this.processBooking();
    }
  }
  
  previousStep(): void {
    if (this.step() > 1) {
      this.step.set(this.step() - 1);
      this.error.set(null);
    }
  }
  
  isValidEmail(email: string): boolean {
    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    return emailRegex.test(email);
  }
  
  isValidCardNumber(): boolean {
    return this.cardDetails.cardNumber.replace(/\s/g, '').length === 16;
  }
  
  isValidExpiryDate(): boolean {
    const expiryRegex = /^(0[1-9]|1[0-2])\/([0-9]{2})$/;
    if (!expiryRegex.test(this.cardDetails.expiryDate)) return false;
    
    const [month, year] = this.cardDetails.expiryDate.split('/');
    const expiryDate = new Date(2000 + parseInt(year), parseInt(month), 0);
    return expiryDate > new Date();
  }
  
  isValidCVV(): boolean {
    return this.cardDetails.cvv.length >= 3 && this.cardDetails.cvv.length <= 4;
  }
  
  processBooking(): void {
    // Validate payment details for non-mock payments
    if (this.paymentMethod !== 'mock') {
      if (!this.isValidCardNumber()) {
        this.error.set('Invalid card number');
        return;
      }
      if (!this.isValidExpiryDate()) {
        this.error.set('Invalid expiry date');
        return;
      }
      if (!this.isValidCVV()) {
        this.error.set('Invalid CVV');
        return;
      }
    }
    
    this.loading.set(true);
    this.error.set(null);
    
    // Simulate payment processing delay
    setTimeout(() => {
      const booking: CreateBookingDto = {
        roomId: this.room!.id,
        customerName: this.formData.customerName,
        customerEmail: this.formData.customerEmail,
        customerPhone: this.formData.customerPhone,
        checkInDate: new Date(this.formData.checkInDate),
        checkOutDate: new Date(this.formData.checkOutDate),
        adults: this.formData.adults,
        children: this.formData.children
      };
      
      this.hotelService.bookRoom(booking).subscribe({
        next: (response) => {
          this.loading.set(false);
          this.step.set(3);
        },
        error: (err) => {
          this.loading.set(false);
          this.error.set('Failed to process booking. Please try again.');
          console.error('Booking error:', err);
        }
      });
    }, 1500);
  }
  
  formatCardNumber(event: Event): void {
    const input = event.target as HTMLInputElement;
    let value = input.value.replace(/\s/g, '');
    if (value.length > 16) value = value.slice(0, 16);
    
    // Add spaces every 4 digits
    const formatted = value.replace(/(\d{4})/g, '$1 ').trim();
    this.cardDetails.cardNumber = formatted;
  }
  
  formatExpiryDate(event: Event): void {
    const input = event.target as HTMLInputElement;
    let value = input.value.replace(/\D/g, '');
    if (value.length > 4) value = value.slice(0, 4);
    
    if (value.length >= 3) {
      value = value.slice(0, 2) + '/' + value.slice(2);
    }
    this.cardDetails.expiryDate = value;
  }
  
  formatCVV(event: Event): void {
    const input = event.target as HTMLInputElement;
    let value = input.value.replace(/\D/g, '');
    if (value.length > 4) value = value.slice(0, 4);
    this.cardDetails.cvv = value;
  }
  
  finishBooking(): void {
    this.onClose();
    this.booked.emit();
  }
}