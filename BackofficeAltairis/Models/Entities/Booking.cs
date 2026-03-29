using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackofficeAltairis.Models.Entities;

[Table("Bookings")]
public class Booking
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    
    [Required]
    public int RoomId { get; set; }
    
    [Required]
    [MaxLength(255)]
    public string CustomerName { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(255)]
    [EmailAddress]
    public string CustomerEmail { get; set; } = string.Empty;
    
    [MaxLength(50)]
    public string? CustomerPhone { get; set; }
    
    [Required]
    public DateTime CheckInDate { get; set; }
    
    [Required]
    public DateTime CheckOutDate { get; set; }
    
    public int Adults { get; set; } = 1;
    
    public int Children { get; set; } = 0;
    
    [Required]
    [Column(TypeName = "decimal(10,2)")]
    public decimal TotalPrice { get; set; }
    
    public string Status { get; set; } = "Pending";
    
    public DateTime BookingDate { get; set; } = DateTime.Now;
    
    // Navigation property
    [ForeignKey("RoomId")]
    public virtual Room? Room { get; set; }
}