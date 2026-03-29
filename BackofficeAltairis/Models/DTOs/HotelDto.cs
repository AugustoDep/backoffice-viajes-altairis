namespace BackofficeAltairis.Models.DTOs;

public class HotelDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Country { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string? MainPhoto { get; set; }
    public int Stars { get; set; }
    public List<RoomDto> Rooms { get; set; } = new();
}

public class CreateHotelDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Country { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string? MainPhoto { get; set; }
    public int Stars { get; set; }
}

public class UpdateHotelDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Country { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string? MainPhoto { get; set; }
    public int Stars { get; set; }
}