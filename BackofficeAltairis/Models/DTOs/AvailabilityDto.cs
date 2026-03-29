namespace BackofficeAltairis.Models.DTOs;

public class MonthlyStatsDto
{
    public string Month { get; set; } = string.Empty;
    public int Year { get; set; }
    public int TotalRooms { get; set; }
    public int BookedRooms { get; set; }
    public int AvailableRooms { get; set; }
    public double OccupancyRate { get; set; }
}

public class UpdateAvailabilityRequestDto
{
    public DateTime Date { get; set; }
    public bool IsAvailable { get; set; }
    public decimal? PriceOverride { get; set; }
}

public class AddBlackoutRequestDto
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string? Reason { get; set; }
}