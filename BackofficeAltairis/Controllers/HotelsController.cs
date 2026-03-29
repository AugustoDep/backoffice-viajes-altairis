using Microsoft.AspNetCore.Mvc;
using BackofficeAltairis.Services.Interfaces;
using BackofficeAltairis.Models.DTOs;

namespace BackofficeAltairis.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HotelsController : ControllerBase
{
    private readonly IHotelService _hotelService; 
    private readonly ILogger<HotelsController> _logger;

    public HotelsController(IHotelService hotelService, ILogger<HotelsController> logger)
    {
        _hotelService = hotelService;
        _logger = logger;
    }
    
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 25,
        [FromQuery] string? searchTerm = null,
        [FromQuery] int? stars = null)
    {
        try
        {
            var (hotels, totalCount) = await _hotelService.GetPaginatedHotelsAsync(
                page, pageSize, searchTerm, stars);
            
            return Ok(new
            {
                hotels = hotels,
                totalCount = totalCount,
                page = page,
                pageSize = pageSize,
                totalPages = (int)Math.Ceiling((double)totalCount / pageSize)
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting hotels");
            return StatusCode(500, new { error = ex.Message });
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            var hotel = await _hotelService.GetHotelByIdAsync(id);
            if (hotel == null)
                return NotFound();
            
            return Ok(hotel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting hotel {Id}", id);
            return StatusCode(500, new { error = ex.Message });
        }
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateHotelDto createHotelDto)
    {
        try
        {
            var created = await _hotelService.CreateHotelAsync(createHotelDto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating hotel");
            return StatusCode(500, new { error = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateHotelDto updateHotelDto)
    {
        try
        {
            if (id != updateHotelDto.Id)
                return BadRequest("ID in URL does not match ID in body");
            
            await _hotelService.UpdateHotelAsync(updateHotelDto);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Hotel {Id} not found for update", id);
            return NotFound(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating hotel {Id}", id);
            return StatusCode(500, new { error = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _hotelService.DeleteHotelAsync(id);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting hotel {Id}", id);
            return StatusCode(500, new { error = ex.Message });
        }
    }

    [HttpGet("{hotelId}/rooms")]
    public async Task<IActionResult> GetRooms(int hotelId)
    {
        try
        {
            var rooms = await _hotelService.GetRoomsByHotelIdAsync(hotelId);
            return Ok(rooms);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting rooms for hotel {HotelId}", hotelId);
            return StatusCode(500, new { error = ex.Message });
        }
    }

    [HttpGet("rooms/available/{hotelId}")]
    public async Task<IActionResult> GetAvailableRooms(int hotelId)
    {
        try
        {
            var rooms = await _hotelService.GetAvailableRoomsByHotelAsync(hotelId);
            return Ok(rooms);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting available rooms for hotel {HotelId}", hotelId);
            return StatusCode(500, new { error = ex.Message });
        }
    }

    [HttpGet("rooms/{roomId}")]
    public async Task<IActionResult> GetRoomById(int roomId)
    {
        try
        {
            var room = await _hotelService.GetRoomByIdAsync(roomId);
            if (room == null)
                return NotFound();
            
            return Ok(room);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting room {RoomId}", roomId);
            return StatusCode(500, new { error = ex.Message });
        }
    }

    [HttpPost("rooms")]
    public async Task<IActionResult> CreateRoom([FromBody] CreateRoomDto createRoomDto)
    {
        try
        {
            var created = await _hotelService.CreateRoomAsync(createRoomDto);
            return CreatedAtAction(nameof(GetRoomById), new { roomId = created.Id }, created);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating room");
            return StatusCode(500, new { error = ex.Message });
        }
    }

    [HttpPut("rooms/{roomId}")]
    public async Task<IActionResult> UpdateRoom(int roomId, [FromBody] UpdateRoomDto updateRoomDto)
    {
        try
        {
            if (roomId != updateRoomDto.Id)
                return BadRequest("ID in URL does not match ID in body");
            
            await _hotelService.UpdateRoomAsync(updateRoomDto);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Room {RoomId} not found for update", roomId);
            return NotFound(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating room {RoomId}", roomId);
            return StatusCode(500, new { error = ex.Message });
        }
    }

    [HttpDelete("rooms/{roomId}")]
    public async Task<IActionResult> DeleteRoom(int roomId)
    {
        try
        {
            await _hotelService.DeleteRoomAsync(roomId);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting room {RoomId}", roomId);
            return StatusCode(500, new { error = ex.Message });
        }
    }

    [HttpPost("book")]
    public async Task<IActionResult> BookRoom([FromBody] CreateBookingDto createBookingDto)
    {
        try
        {
            var result = await _hotelService.BookRoomAsync(createBookingDto.RoomId, createBookingDto);
            
            if (!result)
                return BadRequest(new { error = "Room not available" });
            
            return Ok(new { success = true, message = "Room booked successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error booking room");
            return StatusCode(500, new { error = ex.Message });
        }
    }

    [HttpGet("bookings/room/{roomId}")]
    public async Task<IActionResult> GetBookingsByRoom(int roomId)
    {
        try
        {
            var bookings = await _hotelService.GetBookingsByRoomIdAsync(roomId);
            return Ok(bookings);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting bookings for room {RoomId}", roomId);
            return StatusCode(500, new { error = ex.Message });
        }
    }

    [HttpGet("bookings/customer/{email}")]
    public async Task<IActionResult> GetBookingsByCustomer(string email)
    {
        try
        {
            var bookings = await _hotelService.GetBookingsByCustomerEmailAsync(email);
            return Ok(bookings);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting bookings for customer {Email}", email);
            return StatusCode(500, new { error = ex.Message });
        }
    }

    [HttpGet("bookings/active")]
    public async Task<IActionResult> GetActiveBookings()
    {
        try
        {
            var bookings = await _hotelService.GetActiveBookingsAsync();
            return Ok(bookings);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting active bookings");
            return StatusCode(500, new { error = ex.Message });
        }
    }

    [HttpGet("bookings/{bookingId}")]
    public async Task<IActionResult> GetBookingById(int bookingId)
    {
        try
        {
            var booking = await _hotelService.GetBookingByIdAsync(bookingId);
            if (booking == null)
                return NotFound();
            
            return Ok(booking);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting booking {BookingId}", bookingId);
            return StatusCode(500, new { error = ex.Message });
        }
    }
}