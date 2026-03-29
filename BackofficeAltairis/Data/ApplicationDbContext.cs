using Microsoft.EntityFrameworkCore;
using BackofficeAltairis.Models.Entities;

namespace BackofficeAltairis.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
    
    public DbSet<Hotel> Hotels { get; set; }
    public DbSet<Room> Rooms { get; set; }
    public DbSet<Booking> Bookings { get; set; }
    public DbSet<Log> Logs { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Configure Hotel - Room relationship (One-to-Many)
        modelBuilder.Entity<Room>()
            .HasOne(r => r.Hotel)
            .WithMany(h => h.Rooms)
            .HasForeignKey(r => r.HotelId)
            .OnDelete(DeleteBehavior.Cascade);
        
        // Configure Room - Booking relationship (One-to-Many)
        modelBuilder.Entity<Booking>()
            .HasOne(b => b.Room)
            .WithMany(r => r.Bookings)
            .HasForeignKey(b => b.RoomId)
            .OnDelete(DeleteBehavior.Cascade);
        
        // Configure enum as string
        modelBuilder.Entity<Booking>()
            .Property(b => b.Status)
            .HasConversion<string>();
        
        modelBuilder.Entity<Room>()
            .HasIndex(r => r.HotelId);
        
        
        modelBuilder.Entity<Booking>()
            .HasIndex(b => b.RoomId);
        
        modelBuilder.Entity<Booking>()
            .HasIndex(b => new { b.CheckInDate, b.CheckOutDate });
        
        modelBuilder.Entity<Hotel>()
            .HasIndex(h => h.City);
    }
}