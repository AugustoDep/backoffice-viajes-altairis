using Microsoft.EntityFrameworkCore;
using BackofficeAltairis.Data;
using BackofficeAltairis.Models.Entities;
using BackofficeAltairis.Services.Interfaces;

namespace BackofficeAltairis.Services.Implementations;

public class RoomAvailabilityService : IRoomAvailabilityService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<RoomAvailabilityService> _logger;

    public RoomAvailabilityService(ApplicationDbContext context, ILogger<RoomAvailabilityService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<bool> CheckAvailabilityAsync(int roomId, DateTime checkIn, DateTime checkOut)
    {
        // Check for overlapping confirmed bookings
        var overlappingBookings = await _context.Bookings
            .Where(b => b.RoomId == roomId &&
                        b.Status == "Confirmed" &&
                        b.CheckInDate < checkOut &&
                        b.CheckOutDate > checkIn)
            .AnyAsync();
        
        return !overlappingBookings;
    }

    public async Task<Dictionary<DateTime, bool>> GetAvailabilityAsync(int roomId, DateTime startDate, DateTime endDate)
    {
        var availability = new Dictionary<DateTime, bool>();
        var currentDate = startDate.Date;
        
        // Get all confirmed bookings for this room in the date range
        var bookings = await _context.Bookings
            .Where(b => b.RoomId == roomId &&
                        b.Status == "Confirmed" &&
                        b.CheckInDate < endDate &&
                        b.CheckOutDate > startDate)
            .ToListAsync();
        
        while (currentDate < endDate.Date)
        {
            var isBooked = bookings.Any(b => 
                b.CheckInDate.Date <= currentDate && 
                b.CheckOutDate.Date > currentDate);
            
            availability[currentDate] = !isBooked;
            currentDate = currentDate.AddDays(1);
        }
        
        return availability;
    }

    public async Task<Dictionary<DateTime, decimal>> GetPricesAsync(int roomId, DateTime startDate, DateTime endDate)
    {
        var prices = new Dictionary<DateTime, decimal>();
        var room = await _context.Rooms.FindAsync(roomId);
        
        if (room == null) return prices;
        
        var currentDate = startDate.Date;
        
        while (currentDate < endDate.Date)
        {
            // Simple pricing - just the room's base price
            // Could be enhanced with seasonal pricing if needed
            prices[currentDate] = room.PricePerNight;
            currentDate = currentDate.AddDays(1);
        }
        
        return prices;
    }

    public async Task<decimal> CalculateTotalPriceAsync(int roomId, DateTime checkIn, DateTime checkOut)
    {
        var room = await _context.Rooms.FindAsync(roomId);
        if (room == null) return 0;
        
        var nights = (checkOut - checkIn).Days;
        return room.PricePerNight * nights;
    }

    public async Task<List<DateTime>> GetUnavailableDatesAsync(int roomId, DateTime startDate, DateTime endDate)
    {
        var bookings = await _context.Bookings
            .Where(b => b.RoomId == roomId &&
                        b.Status == "Confirmed" &&
                        b.CheckInDate < endDate &&
                        b.CheckOutDate > startDate)
            .ToListAsync();
        
        var unavailableDates = new List<DateTime>();
        
        foreach (var booking in bookings)
        {
            var currentDate = booking.CheckInDate.Date;
            while (currentDate < booking.CheckOutDate.Date)
            {
                if (currentDate >= startDate && currentDate < endDate)
                {
                    unavailableDates.Add(currentDate);
                }
                currentDate = currentDate.AddDays(1);
            }
        }
        
        return unavailableDates.Distinct().ToList();
    }

    // These methods are no longer needed for the new design
    // Keep them for interface compatibility but implement as no-ops or remove from interface
    public Task UpdateAvailabilityAsync(int roomId, DateTime date, bool isAvailable, decimal? priceOverride = null)
    {
        // Not needed with new design - availability is derived from bookings
        return Task.CompletedTask;
    }

    public Task BulkUpdateAvailabilityAsync(int roomId, DateTime startDate, DateTime endDate, bool isAvailable)
    {
        // Not needed with new design
        return Task.CompletedTask;
    }
}