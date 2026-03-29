using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackofficeAltairis.Models.Entities;

[Table("Hotels")]
public class Hotel
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    
    [Required]
    [MaxLength(255)]
    public string Name { get; set; } = string.Empty;
    
    public string? Description { get; set; }
    
    [Required]
    [MaxLength(100)]
    public string Country { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(100)]
    public string City { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(255)]
    public string Address { get; set; } = string.Empty;
    
    [MaxLength(500)]
    public string? MainPhoto { get; set; }
    
    public int Stars { get; set; } = 3;
    
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
    
    // Navigation property: A hotel can have many rooms
    public virtual ICollection<Room> Rooms { get; set; } = new List<Room>();
}