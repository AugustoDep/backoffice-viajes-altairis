using BackofficeAltairis.Models.Entities;

namespace BackofficeAltairis.Data;

public static class SeedData
{
    public static async Task InitializeAsync(ApplicationDbContext context)
    {
        if (context.Hotels.Any())
            return;

        var hotels = new List<Hotel>
        {
            new Hotel 
            { 
                Name = "Ocean Paradise Resort", 
                Description = "Luxury beachfront resort with stunning ocean views", 
                Country = "Mexico", 
                City = "Cancun", 
                Address = "Boulevard Kukulcan Km 14.5", 
                Stars = 5,
                MainPhoto = "https://images.unsplash.com/photo-1566073771259-6a8506099945",
                Rooms = new List<Room>
                {
                    new Room { Type = "Deluxe Suite", PricePerNight = 350.00m, Description = "Spacious suite with ocean view", Photo = "https://images.unsplash.com/photo-1582719508461-905c673771fd" },
                    new Room { Type = "Standard Double", PricePerNight = 180.00m, Description = "Comfortable room", Photo = "https://images.unsplash.com/photo-1582719508461-905c673771fd" }
                }
            },
            new Hotel 
            { 
                Name = "Mountain View Hotel", 
                Description = "Cozy hotel with breathtaking mountain views", 
                Country = "Switzerland", 
                City = "Interlaken", 
                Address = "Höheweg 41", 
                Stars = 4,
                MainPhoto = "https://images.unsplash.com/photo-1582719508461-905c673771fd",
                Rooms = new List<Room>
                {
                    new Room { Type = "Mountain View Double", PricePerNight = 250.00m, Description = "Room with mountain views" },
                    new Room { Type = "Economy Single", PricePerNight = 120.00m, Description = "Cozy single room" }
                }
            }
        };

        await context.Hotels.AddRangeAsync(hotels);
        await context.SaveChangesAsync();
    }
}