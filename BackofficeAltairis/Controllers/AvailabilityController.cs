using Microsoft.AspNetCore.Mvc;
using BackofficeAltairis.Services.Interfaces;
using BackofficeAltairis.Models.DTOs;
using BackofficeAltairis.Data;
using Microsoft.EntityFrameworkCore;

namespace BackofficeAltairis.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AvailabilityController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IRoomAvailabilityService _availabilityService;
    private readonly ILogger<AvailabilityController> _logger;

    public AvailabilityController(
        ApplicationDbContext context,
        IRoomAvailabilityService availabilityService, 
        ILogger<AvailabilityController> logger)
    {
        _context = context;
        _availabilityService = availabilityService;
        _logger = logger;
    }
    
    [HttpGet("dashboard")]
    public async Task<IActionResult> GetDashboardData(
        [FromQuery] int? year = null,
        [FromQuery] string? country = null,
        [FromQuery] string? city = null)
    {
        try
        {
            // Debug logs
            Console.WriteLine($"=== DASHBOARD REQUEST ===");
            Console.WriteLine($"Year: {year}, Country: {country}, City: {city}");
            
            // Get all hotels (for filters)
            var hotelsQuery = _context.Hotels.AsQueryable();
            
            // Apply country filter
            if (!string.IsNullOrEmpty(country))
            {
                hotelsQuery = hotelsQuery.Where(h => h.Country == country);
            }
            
            // Apply city filter
            if (!string.IsNullOrEmpty(city))
            {
                hotelsQuery = hotelsQuery.Where(h => h.City == city);
            }
            
            var hotels = await hotelsQuery
                .Select(h => new
                {
                    h.Id,
                    h.Name,
                    h.City,
                    h.Country,
                    h.Stars
                })
                .ToListAsync();
            
            // Get hotel IDs for filtering rooms and bookings
            var hotelIds = hotels.Select(h => h.Id).ToList();
            
            // If no hotels match filters, return empty data
            if (!hotelIds.Any())
            {
                return Ok(new
                {
                    Hotels = hotels,
                    MonthlyStats = new List<MonthlyStatsDto>(),
                    AvailableYears = new List<int>(),
                    Summary = new
                    {
                        TotalHotels = 0,
                        TotalRooms = 0
                    }
                });
            }
            
            // Get rooms filtered by hotel IDs
            var roomsQuery = _context.Rooms.Where(r => hotelIds.Contains(r.HotelId));
            var totalRooms = await roomsQuery.CountAsync();
            
            // Get room IDs for filtering bookings
            var roomIds = await roomsQuery.Select(r => r.Id).ToListAsync();
            
            // Determine the target year (default to current year if not specified)
            var targetYear = year ?? DateTime.Now.Year;
            
            // Get all confirmed bookings for rooms in the filtered hotels
            var allBookings = await _context.Bookings
                .Where(b => b.Status == "Confirmed" && 
                            roomIds.Contains(b.RoomId))
                .Select(b => new { b.CheckInDate, b.CheckOutDate })
                .ToListAsync();
            
            // Build monthly stats for the specified year (all 12 months)
            var monthlyStats = new List<MonthlyStatsDto>();
            
            for (int month = 1; month <= 12; month++)
            {
                var monthName = new DateTime(targetYear, month, 1).ToString("MMM");
                var monthStart = new DateTime(targetYear, month, 1);
                var monthEnd = monthStart.AddMonths(1);
                
                // Count rooms that are occupied during this month
                // A room is occupied if the booking period overlaps with the month
                var bookedRooms = allBookings
                    .Count(b => b.CheckInDate < monthEnd && b.CheckOutDate > monthStart);
                
                var occupancyRate = totalRooms > 0 ? (double)bookedRooms / totalRooms * 100 : 0;
                
                monthlyStats.Add(new MonthlyStatsDto
                {
                    Month = monthName,
                    Year = targetYear,
                    TotalRooms = totalRooms,
                    BookedRooms = bookedRooms,
                    AvailableRooms = totalRooms - bookedRooms,
                    OccupancyRate = Math.Round(occupancyRate, 1)
                });
            }
            
            // Get all available years from bookings (filtered by country/city)
            var allAvailableYears = await _context.Bookings
                .Where(b => b.Status == "Confirmed" && 
                            roomIds.Contains(b.RoomId))
                .Select(b => b.CheckInDate.Year)
                .Distinct()
                .ToListAsync();
            
            // Add current year if not present
            var currentYear = DateTime.Now.Year;
            if (!allAvailableYears.Contains(currentYear))
            {
                allAvailableYears.Add(currentYear);
            }
            allAvailableYears = allAvailableYears.OrderBy(y => y).ToList();
            
            var result = new
            {
                Hotels = hotels,
                MonthlyStats = monthlyStats,
                AvailableYears = allAvailableYears,
                Summary = new
                {
                    TotalHotels = hotels.Count,
                    TotalRooms = totalRooms
                }
            };
            
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting dashboard data");
            return StatusCode(500, new { error = ex.Message });
        }
    }
    
    [HttpGet("rooms")]
    public async Task<IActionResult> GetRoomsAvailability(
        [FromQuery] int hotelId,
        [FromQuery] DateTime checkIn,
        [FromQuery] DateTime checkOut)
    {
        try
        {
            // Validate dates
            if (checkIn >= checkOut)
            {
                return BadRequest(new { error = "Check-out date must be after check-in date" });
            }
            
            // Get all rooms for this hotel
            var rooms = await _context.Rooms
                .Where(r => r.HotelId == hotelId)
                .ToListAsync();
            
            if (!rooms.Any())
            {
                return Ok(new List<object>());
            }
            
            var results = new List<object>();
            
            foreach (var room in rooms)
            {
                // Check if room has any overlapping confirmed bookings
                var isAvailable = !await _context.Bookings
                    .AnyAsync(b => b.RoomId == room.Id &&
                                b.Status == "Confirmed" &&
                                b.CheckInDate < checkOut &&
                                b.CheckOutDate > checkIn);
                
                results.Add(new
                {
                    roomId = room.Id,
                    isAvailable = isAvailable,
                    price = room.PricePerNight
                });
            }
            
            return Ok(results);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking room availability for hotel {HotelId}", hotelId);
            return StatusCode(500, new { error = ex.Message });
        }
    }

    [HttpGet("rooms/{roomId}")]
    public async Task<IActionResult> GetSingleRoomAvailability(
        int roomId,
        [FromQuery] DateTime checkIn,
        [FromQuery] DateTime checkOut)
    {
        try
        {
            // Validate dates
            if (checkIn >= checkOut)
            {
                return BadRequest(new { error = "Check-out date must be after check-in date" });
            }
            
            var room = await _context.Rooms.FindAsync(roomId);
            if (room == null)
            {
                return NotFound(new { error = $"Room with ID {roomId} not found" });
            }
            
            var isAvailable = !await _context.Bookings
                .AnyAsync(b => b.RoomId == roomId &&
                            b.Status == "Confirmed" &&
                            b.CheckInDate < checkOut &&
                            b.CheckOutDate > checkIn);
            
            return Ok(new
            {
                roomId = room.Id,
                isAvailable = isAvailable,
                price = room.PricePerNight
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking room availability for room {RoomId}", roomId);
            return StatusCode(500, new { error = ex.Message });
        }
    }
}