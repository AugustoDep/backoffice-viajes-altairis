using BackofficeAltairis.Models.Entities;

namespace BackofficeAltairis.Services.Interfaces;

public interface IRoomAvailabilityService
{
    Task<bool> CheckAvailabilityAsync(int roomId, DateTime checkIn, DateTime checkOut);
    Task<Dictionary<DateTime, bool>> GetAvailabilityAsync(int roomId, DateTime startDate, DateTime endDate);
    Task<Dictionary<DateTime, decimal>> GetPricesAsync(int roomId, DateTime startDate, DateTime endDate);
    Task<decimal> CalculateTotalPriceAsync(int roomId, DateTime checkIn, DateTime checkOut);
    Task<List<DateTime>> GetUnavailableDatesAsync(int roomId, DateTime startDate, DateTime endDate);
    
    // Optional methods (can be removed if not needed)
    Task UpdateAvailabilityAsync(int roomId, DateTime date, bool isAvailable, decimal? priceOverride = null);
    Task BulkUpdateAvailabilityAsync(int roomId, DateTime startDate, DateTime endDate, bool isAvailable);
}