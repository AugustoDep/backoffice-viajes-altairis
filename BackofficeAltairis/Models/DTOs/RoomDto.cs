namespace BackofficeAltairis.Models.DTOs;

public class RoomDto
{
    public int Id { get; set; }
    public int HotelId { get; set; }
    public string Type { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal PricePerNight { get; set; }
    public string? Photo { get; set; }
    public bool IsAvailable { get; set; } = true;
}

public class CreateRoomDto
{
    public int HotelId { get; set; }
    public string Type { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal PricePerNight { get; set; }
    public string? Photo { get; set; }
}

public class UpdateRoomDto
{
    public int Id { get; set; }
    public string Type { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal PricePerNight { get; set; }
    public string? Photo { get; set; }
}