## Database Setup

1. **Install MySQL** (if not already installed)
2. **Update connection string** in `appsettings.Development.json`:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=localhost;Port=3306;Database=HotelBookingDB;User=root;Password=YOUR_PASSWORD;"
     }
   }