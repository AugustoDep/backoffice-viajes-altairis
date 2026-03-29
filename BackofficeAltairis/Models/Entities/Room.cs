using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackofficeAltairis.Models.Entities;

[Table("Rooms")]
public class Room
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    
    [Required]
    public int HotelId { get; set; }
    
    [Required]
    [MaxLength(100)]
    public string Type { get; set; } = string.Empty;
    
    public string? Description { get; set; }
    
    [Required]
    [Column(TypeName = "decimal(10,2)")]
    public decimal PricePerNight { get; set; }
    
    [MaxLength(500)]
    public string? Photo { get; set; }
        
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
    
    [ForeignKey("HotelId")]
    public virtual Hotel? Hotel { get; set; }
    
    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();
}