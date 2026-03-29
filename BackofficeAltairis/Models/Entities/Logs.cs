using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackofficeAltairis.Models.Entities;

[Table("Logs")]
public class Log
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    
    [Required]
    [MaxLength(50)]
    public string Action { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(100)]
    public string Entity { get; set; } = string.Empty; 
    
    public int EntityId { get; set; }
    
    [MaxLength(255)]
    public string EntityName { get; set; } = string.Empty; 
    
    [MaxLength(500)]
    public string? Details { get; set; }
    
    [Required]
    [MaxLength(100)]
    public string User { get; set; } = "System";
    
    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}