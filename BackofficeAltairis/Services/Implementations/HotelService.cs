using Microsoft.EntityFrameworkCore;
using BackofficeAltairis.Data;
using BackofficeAltairis.Models.Entities;
using BackofficeAltairis.Models.DTOs;
using BackofficeAltairis.Services.Interfaces;

namespace BackofficeAltairis.Services.Implementations;

public class HotelService : IHotelService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<HotelService> _logger;
    private readonly ILogService _logService;
    private readonly IRoomAvailabilityService _availabilityService;

    public HotelService(
        ApplicationDbContext context, 
        ILogger<HotelService> logger,
        ILogService logService,
        IRoomAvailabilityService availabilityService)
    {
        _context = context;
        _logger = logger;
        _logService = logService;
        _availabilityService = availabilityService;
    }

    // Hotel operations with DTOs
    public async Task<List<HotelDto>> GetAllHotelsAsync()
    {
        return await _context.Hotels
            .Include(h => h.Rooms)
            .Select(h => new HotelDto
            {
                Id = h.Id,
                Name = h.Name,
                Description = h.Description,
                Country = h.Country,
                City = h.City,
                Address = h.Address,
                MainPhoto = h.MainPhoto,
                Stars = h.Stars,
                Rooms = h.Rooms.Select(r => new RoomDto
                {
                    Id = r.Id,
                    HotelId = r.HotelId,
                    Type = r.Type,
                    Description = r.Description,
                    PricePerNight = r.PricePerNight,
                    Photo = r.Photo,
                }).ToList()
            })
            .ToListAsync();
    }

    public async Task<HotelDto?> GetHotelByIdAsync(int id)
    {
        var hotel = await _context.Hotels
            .Include(h => h.Rooms)
            .FirstOrDefaultAsync(h => h.Id == id);
            
        if (hotel == null) return null;
        
        return new HotelDto
        {
            Id = hotel.Id,
            Name = hotel.Name,
            Description = hotel.Description,
            Country = hotel.Country,
            City = hotel.City,
            Address = hotel.Address,
            MainPhoto = hotel.MainPhoto,
            Stars = hotel.Stars,
            Rooms = hotel.Rooms.Select(r => new RoomDto
            {
                Id = r.Id,
                HotelId = r.HotelId,
                Type = r.Type,
                Description = r.Description,
                PricePerNight = r.PricePerNight,
                Photo = r.Photo,
            }).ToList()
        };
    }

    public async Task<HotelDto> CreateHotelAsync(CreateHotelDto createHotelDto)
    {
        var hotel = new Hotel
        {
            Name = createHotelDto.Name,
            Description = createHotelDto.Description,
            Country = createHotelDto.Country,
            City = createHotelDto.City,
            Address = createHotelDto.Address,
            MainPhoto = createHotelDto.MainPhoto,
            Stars = createHotelDto.Stars,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        };
        
        await _context.Hotels.AddAsync(hotel);
        await _context.SaveChangesAsync();

        await _logService.LogAsync(
        "CREATE", 
        "Hotel", 
        hotel.Id, 
        hotel.Name, 
        $"Created hotel with {hotel.Stars} stars in {hotel.City}",
        "System"
        );
        
        return new HotelDto
        {
            Id = hotel.Id,
            Name = hotel.Name,
            Description = hotel.Description,
            Country = hotel.Country,
            City = hotel.City,
            Address = hotel.Address,
            MainPhoto = hotel.MainPhoto,
            Stars = hotel.Stars,
            Rooms = new List<RoomDto>()
        };
    }

    public async Task UpdateHotelAsync(UpdateHotelDto updateHotelDto)
    {
        var hotel = await _context.Hotels.FindAsync(updateHotelDto.Id);
        if (hotel == null)
            throw new KeyNotFoundException($"Hotel with ID {updateHotelDto.Id} not found");
        
        var changes = new List<string>();
        if (hotel.Name != updateHotelDto.Name) changes.Add($"Name: '{hotel.Name}' → '{updateHotelDto.Name}'");
        if (hotel.Stars != updateHotelDto.Stars) changes.Add($"Stars: {hotel.Stars} → {updateHotelDto.Stars}");
        if (hotel.City != updateHotelDto.City) changes.Add($"City: '{hotel.City}' → '{updateHotelDto.City}'");
        
        hotel.Name = updateHotelDto.Name;
        hotel.Description = updateHotelDto.Description;
        hotel.Country = updateHotelDto.Country;
        hotel.City = updateHotelDto.City;
        hotel.Address = updateHotelDto.Address;
        hotel.MainPhoto = updateHotelDto.MainPhoto;
        hotel.Stars = updateHotelDto.Stars;
        hotel.UpdatedAt = DateTime.Now;
        
        await _context.SaveChangesAsync();

        await _logService.LogAsync(
            "UPDATE", 
            "Hotel", 
            hotel.Id, 
            hotel.Name, 
            changes.Any() ? $"Changes: {string.Join(", ", changes)}" : "No significant changes",
            "System"
        );
    }

    public async Task DeleteHotelAsync(int id)
    {
        var hotel = await _context.Hotels.FindAsync(id);
        if (hotel != null)
        {
            // Store hotel info before deletion for logging
            var hotelName = hotel.Name;
            var hotelStars = hotel.Stars;
            var hotelCity = hotel.City;
            
            _context.Hotels.Remove(hotel);
            await _context.SaveChangesAsync();
            
            // Log the deletion
            await _logService.LogAsync(
                "DELETE",
                "Hotel",
                id,
                hotelName,
                $"Deleted hotel '{hotelName}' ({hotelStars} stars) in {hotelCity}",
                "System"
            );
        }
    }

    // Room operations with DTOs
    public async Task<List<RoomDto>> GetRoomsByHotelIdAsync(int hotelId)
    {
        return await _context.Rooms
            .Where(r => r.HotelId == hotelId)
            .Select(r => new RoomDto
            {
                Id = r.Id,
                HotelId = r.HotelId,
                Type = r.Type,
                Description = r.Description,
                PricePerNight = r.PricePerNight,
                Photo = r.Photo,
            })
            .ToListAsync();
    }

    public async Task<List<RoomDto>> GetAvailableRoomsByHotelAsync(int hotelId)
    {
        return await _context.Rooms
            .Where(r => r.HotelId == hotelId)
            .Select(r => new RoomDto
            {
                Id = r.Id,
                HotelId = r.HotelId,
                Type = r.Type,
                Description = r.Description,
                PricePerNight = r.PricePerNight,
                Photo = r.Photo,
            })
            .ToListAsync();
    }

    public async Task<RoomDto?> GetRoomByIdAsync(int id)
    {
        var room = await _context.Rooms
            .FirstOrDefaultAsync(r => r.Id == id);
            
        if (room == null) return null;
        
        return new RoomDto
        {
            Id = room.Id,
            HotelId = room.HotelId,
            Type = room.Type,
            Description = room.Description,
            PricePerNight = room.PricePerNight,
            Photo = room.Photo,
        };
    }

    public async Task<RoomDto> CreateRoomAsync(CreateRoomDto createRoomDto)
    {
        var room = new Room
        {
            HotelId = createRoomDto.HotelId,
            Type = createRoomDto.Type,
            Description = createRoomDto.Description,
            PricePerNight = createRoomDto.PricePerNight,
            Photo = createRoomDto.Photo,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        };
        
        await _context.Rooms.AddAsync(room);
        await _context.SaveChangesAsync();
        
        return new RoomDto
        {
            Id = room.Id,
            HotelId = room.HotelId,
            Type = room.Type,
            Description = room.Description,
            PricePerNight = room.PricePerNight,
            Photo = room.Photo,
        };
    }

    public async Task UpdateRoomAsync(UpdateRoomDto updateRoomDto)
    {
        var room = await _context.Rooms.FindAsync(updateRoomDto.Id);
        if (room == null)
            throw new KeyNotFoundException($"Room with ID {updateRoomDto.Id} not found");
        
        room.Type = updateRoomDto.Type;
        room.Description = updateRoomDto.Description;
        room.PricePerNight = updateRoomDto.PricePerNight;
        room.Photo = updateRoomDto.Photo;
        room.UpdatedAt = DateTime.Now;
        
        await _context.SaveChangesAsync();
    }

    public async Task DeleteRoomAsync(int id)
    {
        var room = await _context.Rooms.FindAsync(id);
        if (room != null)
        {
            _context.Rooms.Remove(room);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> BookRoomAsync(int roomId, CreateBookingDto createBookingDto)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        
        try
        {
            var room = await _context.Rooms
                .Include(r => r.Hotel)
                .FirstOrDefaultAsync(r => r.Id == roomId);
                
            if (room == null)
                return false;
            
            // Check availability for the entire stay
            var isAvailable = await _availabilityService.CheckAvailabilityAsync(
                roomId, 
                createBookingDto.CheckInDate, 
                createBookingDto.CheckOutDate);
            
            if (!isAvailable)
                return false;
            
            // Calculate total price
            var nights = (createBookingDto.CheckOutDate - createBookingDto.CheckInDate).Days;
            var totalPrice = room.PricePerNight * nights;
            
            // Create booking
            var booking = new Booking
            {
                RoomId = roomId,
                CustomerName = createBookingDto.CustomerName,
                CustomerEmail = createBookingDto.CustomerEmail,
                CustomerPhone = createBookingDto.CustomerPhone,
                CheckInDate = createBookingDto.CheckInDate,
                CheckOutDate = createBookingDto.CheckOutDate,
                Adults = createBookingDto.Adults,
                Children = createBookingDto.Children,
                TotalPrice = totalPrice,
                Status = "Confirmed",
                BookingDate = DateTime.Now
            };
            
            await _context.Bookings.AddAsync(booking);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
            
            // Log the booking
            await _logService.LogAsync(
                "BOOK",
                "Room",
                room.Id,
                $"{room.Type} at {room.Hotel?.Name}",
                $"Booked by {createBookingDto.CustomerName} for {nights} nights, Total: ${totalPrice}",
                createBookingDto.CustomerEmail);
            
            return true;
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError(ex, "Error booking room {RoomId}", roomId);
            throw;
        }
    }    
    
    private async Task<bool> CheckRoomAvailabilityAsync(int roomId, DateTime checkIn, DateTime checkOut)
    {
        // Check if there are any overlapping confirmed bookings for this room
        var overlappingBookings = await _context.Bookings
            .Where(b => b.RoomId == roomId &&
                        b.Status == "Confirmed" &&
                        b.CheckInDate < checkOut &&
                        b.CheckOutDate > checkIn)
            .AnyAsync();
        
        return !overlappingBookings;
    }

    // public async Task<bool> CancelBookingAsync(int bookingId)
    // {
    //     using var transaction = await _context.Database.BeginTransactionAsync();
        
    //     try
    //     {
    //         var booking = await _context.Bookings
    //             .Include(b => b.Room)
    //             .FirstOrDefaultAsync(b => b.Id == bookingId);
                
    //         if (booking == null)
    //             return false;
            
    //         booking.Status = "Cancelled";
            
    //         // Free up the dates
    //         var currentDate = booking.CheckInDate.Date;
    //         while (currentDate < booking.CheckOutDate.Date)
    //         {
    //             await _availabilityService.UpdateAvailabilityAsync(booking.RoomId, currentDate, true);
    //             currentDate = currentDate.AddDays(1);
    //         }
            
    //         await _context.SaveChangesAsync();
    //         await transaction.CommitAsync();
                
    //         return true;
    //     }
    //     catch (Exception ex)
    //     {
    //         await transaction.RollbackAsync();
    //         _logger.LogError(ex, "Error cancelling booking {BookingId}", bookingId);
    //         throw;
    //     }
    // }

    public async Task<List<BookingDto>> GetBookingsByRoomIdAsync(int roomId)
    {
        return await _context.Bookings
            .Where(b => b.RoomId == roomId)
            .Include(b => b.Room)
            .ThenInclude(r => r.Hotel)
            .OrderByDescending(b => b.BookingDate)
            .Select(b => new BookingDto
            {
                Id = b.Id,
                RoomId = b.RoomId,
                RoomType = b.Room != null ? b.Room.Type : string.Empty,
                HotelName = b.Room != null && b.Room.Hotel != null ? b.Room.Hotel.Name : string.Empty,
                CustomerName = b.CustomerName,
                CustomerEmail = b.CustomerEmail,
                CustomerPhone = b.CustomerPhone,
                CheckInDate = b.CheckInDate,
                CheckOutDate = b.CheckOutDate,
                Adults = b.Adults,
                Children = b.Children,
                TotalPrice = b.TotalPrice,
                Status = b.Status,
                BookingDate = b.BookingDate
            })
            .ToListAsync();
    }

    public async Task<List<BookingDto>> GetBookingsByCustomerEmailAsync(string email)
    {
        return await _context.Bookings
            .Where(b => b.CustomerEmail == email)
            .Include(b => b.Room)
            .ThenInclude(r => r.Hotel)
            .OrderByDescending(b => b.BookingDate)
            .Select(b => new BookingDto
            {
                Id = b.Id,
                RoomId = b.RoomId,
                RoomType = b.Room != null ? b.Room.Type : string.Empty,
                HotelName = b.Room != null && b.Room.Hotel != null ? b.Room.Hotel.Name : string.Empty,
                CustomerName = b.CustomerName,
                CustomerEmail = b.CustomerEmail,
                CustomerPhone = b.CustomerPhone,
                CheckInDate = b.CheckInDate,
                CheckOutDate = b.CheckOutDate,
                Adults = b.Adults,
                Children = b.Children,
                TotalPrice = b.TotalPrice,
                Status = b.Status,
                BookingDate = b.BookingDate
            })
            .ToListAsync();
    }

    public async Task<BookingDto?> GetBookingByIdAsync(int id)
    {
        var booking = await _context.Bookings
            .Include(b => b.Room)
            .ThenInclude(r => r.Hotel)
            .FirstOrDefaultAsync(b => b.Id == id);
            
        if (booking == null) return null;
        
        return new BookingDto
        {
            Id = booking.Id,
            RoomId = booking.RoomId,
            RoomType = booking.Room != null ? booking.Room.Type : string.Empty,
            HotelName = booking.Room != null && booking.Room.Hotel != null ? booking.Room.Hotel.Name : string.Empty,
            CustomerName = booking.CustomerName,
            CustomerEmail = booking.CustomerEmail,
            CustomerPhone = booking.CustomerPhone,
            CheckInDate = booking.CheckInDate,
            CheckOutDate = booking.CheckOutDate,
            Adults = booking.Adults,
            Children = booking.Children,
            TotalPrice = booking.TotalPrice,
            Status = booking.Status,
            BookingDate = booking.BookingDate
        };
    }

    public async Task<List<BookingDto>> GetActiveBookingsAsync()
    {
        return await _context.Bookings
            .Where(b => b.Status == "Confirmed" || b.Status == "Pending")
            .Include(b => b.Room)
            .ThenInclude(r => r.Hotel)
            .OrderBy(b => b.CheckInDate)
            .Select(b => new BookingDto
            {
                Id = b.Id,
                RoomId = b.RoomId,
                RoomType = b.Room != null ? b.Room.Type : string.Empty,
                HotelName = b.Room != null && b.Room.Hotel != null ? b.Room.Hotel.Name : string.Empty,
                CustomerName = b.CustomerName,
                CustomerEmail = b.CustomerEmail,
                CustomerPhone = b.CustomerPhone,
                CheckInDate = b.CheckInDate,
                CheckOutDate = b.CheckOutDate,
                Adults = b.Adults,
                Children = b.Children,
                TotalPrice = b.TotalPrice,
                Status = b.Status,
                BookingDate = b.BookingDate
            })
            .ToListAsync();
    }

    public async Task<(List<HotelDto> Hotels, int TotalCount)> GetPaginatedHotelsAsync(
        int page, 
        int pageSize, 
        string? searchTerm = null, 
        int? starFilter = null)
    {
        var query = _context.Hotels.AsQueryable();
        
        // Apply search filter
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var search = searchTerm.ToLower();
            query = query.Where(h => 
                h.Name.ToLower().Contains(search) ||
                h.City.ToLower().Contains(search) ||
                h.Country.ToLower().Contains(search));
        }
        
        // Apply star filter
        if (starFilter.HasValue)
        {
            query = query.Where(h => h.Stars == starFilter.Value);
        }
        
        // Get total count before pagination
        var totalCount = await query.CountAsync();
        
        // Apply pagination
        var hotels = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Include(h => h.Rooms)
            .Select(h => new HotelDto
            {
                Id = h.Id,
                Name = h.Name,
                Description = h.Description,
                Country = h.Country,
                City = h.City,
                Address = h.Address,
                MainPhoto = h.MainPhoto,
                Stars = h.Stars,
                Rooms = h.Rooms.Select(r => new RoomDto
                {
                    Id = r.Id,
                    HotelId = r.HotelId,
                    Type = r.Type,
                    Description = r.Description,
                    PricePerNight = r.PricePerNight,
                    Photo = r.Photo,
                }).ToList()
            })
            .ToListAsync();
        
        return (hotels, totalCount);
    }
}