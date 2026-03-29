import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface Hotel {
  id: number;
  name: string;
  description?: string;
  country: string;
  city: string;
  address: string;
  mainPhoto?: string;
  stars: number;
  rooms: Room[];
}

export interface Room {
  id: number;
  hotelId: number;
  type: string;
  description?: string;
  pricePerNight: number;
  photo?: string;
}

export interface CreateHotelDto {
  name: string;
  description?: string;
  country: string;
  city: string;
  address: string;
  mainPhoto?: string;
  stars: number;
}

export interface CreateBookingDto {
  roomId: number;
  customerName: string;
  customerEmail: string;
  customerPhone?: string;
  checkInDate: Date;
  checkOutDate: Date;
  adults: number;
  children: number;
}

// New interfaces for availability checking
export interface RoomAvailabilityResponse {
  roomId: number;
  isAvailable: boolean;
  price: number;
  unavailableDates?: string[];
}

@Injectable({
  providedIn: 'root'
})
export class HotelService {
  private apiUrl = 'http://localhost:5120/api/hotels';
  private availabilityApiUrl = 'http://localhost:5120/api/availability';

  constructor(private http: HttpClient) { }

  getAllHotels(page: number = 1, pageSize: number = 25, searchTerm?: string, stars?: number): Observable<any> {
    let url = `${this.apiUrl}?page=${page}&pageSize=${pageSize}`;
    
    if (searchTerm) {
      url += `&searchTerm=${encodeURIComponent(searchTerm)}`;
    }
    if (stars) {
      url += `&stars=${stars}`;
    }
    
    return this.http.get<any>(url);
  }

  getHotelById(id: number): Observable<Hotel> {
    return this.http.get<Hotel>(`${this.apiUrl}/${id}`);
  }

  createHotel(hotel: CreateHotelDto): Observable<Hotel> {
    return this.http.post<Hotel>(this.apiUrl, hotel);
  }

  updateHotel(id: number, hotel: any): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${id}`, hotel);
  }

  deleteHotel(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }

  getRoomsByHotel(hotelId: number): Observable<Room[]> {
    return this.http.get<Room[]>(`${this.apiUrl}/${hotelId}/rooms`);
  }

  getAvailableRooms(hotelId: number): Observable<Room[]> {
    return this.http.get<Room[]>(`${this.apiUrl}/rooms/available/${hotelId}`);
  }

  bookRoom(booking: CreateBookingDto): Observable<any> {
    return this.http.post(`${this.apiUrl}/book`, booking);
  }

  getPaginatedHotels(page: number, pageSize: number, searchTerm?: string, stars?: number): Observable<any> {
    let url = `${this.apiUrl}?page=${page}&pageSize=${pageSize}`;
    
    if (searchTerm && searchTerm.trim() !== '') {
      url += `&searchTerm=${encodeURIComponent(searchTerm)}`;
    }
    if (stars) {
      url += `&stars=${stars}`;
    }
    
    return this.http.get<any>(url);
  }

  // ========== NEW AVAILABILITY METHODS ==========
  
  /**
   * Check availability for all rooms in a hotel for given date range
   * @param hotelId - ID of the hotel
   * @param checkIn - Check-in date
   * @param checkOut - Check-out date
   * @returns Observable with array of room availability responses
   */
  checkRoomsAvailability(hotelId: number, checkIn: Date, checkOut: Date): Observable<RoomAvailabilityResponse[]> {
    const params = {
      hotelId: hotelId.toString(),
      checkIn: checkIn.toISOString().split('T')[0],
      checkOut: checkOut.toISOString().split('T')[0]
    };
    return this.http.get<RoomAvailabilityResponse[]>(`${this.availabilityApiUrl}/rooms`, { params });
  }

  /**
   * Check availability for a single room for given date range
   * @param roomId - ID of the room
   * @param checkIn - Check-in date
   * @param checkOut - Check-out date
   * @returns Observable with room availability response
   */
  checkRoomAvailability(roomId: number, checkIn: Date, checkOut: Date): Observable<RoomAvailabilityResponse> {
    const params = {
      checkIn: checkIn.toISOString().split('T')[0],
      checkOut: checkOut.toISOString().split('T')[0]
    };
    return this.http.get<RoomAvailabilityResponse>(`${this.availabilityApiUrl}/rooms/${roomId}`, { params });
  }
}