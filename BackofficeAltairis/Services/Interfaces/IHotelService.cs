using BackofficeAltairis.Models.DTOs;

namespace BackofficeAltairis.Services.Interfaces;

public interface IHotelService
{
    // Hotel operations
    Task<List<HotelDto>> GetAllHotelsAsync();
    Task<HotelDto?> GetHotelByIdAsync(int id);
    Task<HotelDto> CreateHotelAsync(CreateHotelDto createHotelDto);
    Task UpdateHotelAsync(UpdateHotelDto updateHotelDto);
    Task DeleteHotelAsync(int id);
    
    // Room operations
    Task<List<RoomDto>> GetRoomsByHotelIdAsync(int hotelId);
    Task<List<RoomDto>> GetAvailableRoomsByHotelAsync(int hotelId);
    Task<RoomDto?> GetRoomByIdAsync(int id);
    Task<RoomDto> CreateRoomAsync(CreateRoomDto createRoomDto);
    Task UpdateRoomAsync(UpdateRoomDto updateRoomDto);
    Task DeleteRoomAsync(int id);
    
    // Booking operations
    Task<List<BookingDto>> GetBookingsByRoomIdAsync(int roomId);
    Task<List<BookingDto>> GetBookingsByCustomerEmailAsync(string email);
    Task<BookingDto?> GetBookingByIdAsync(int id);
    Task<bool> BookRoomAsync(int roomId, CreateBookingDto createBookingDto);
    // Task<bool> CancelBookingAsync(int bookingId);
    Task<List<BookingDto>> GetActiveBookingsAsync();

    // Add this method to IHotelService
    Task<(List<HotelDto> Hotels, int TotalCount)> GetPaginatedHotelsAsync(
        int page, 
        int pageSize, 
        string? searchTerm = null, 
        int? starFilter = null);
}