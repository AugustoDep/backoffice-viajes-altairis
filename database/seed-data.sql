-- database/seed-data.sql
USE backoffice_altairis;

-- =====================================================
-- HOTELS
-- =====================================================
INSERT INTO Hotels (Name, Description, Country, City, Address, Stars) VALUES
('Ocean Paradise Resort', 'Luxury beachfront resort with stunning ocean views', 'Mexico', 'Cancun', 'Boulevard Kukulcan Km 14.5', 5),
('Mountain View Hotel', 'Cozy hotel with breathtaking mountain views', 'Switzerland', 'Interlaken', 'Höheweg 41', 4),
('City Central Inn', 'Modern hotel in the heart of the city', 'Spain', 'Barcelona', 'La Rambla 128', 3),
('Mayan Palace', 'Traditional Mexican architecture with modern amenities', 'Mexico', 'Playa del Carmen', 'Av. 5ta Norte 123', 4),
('Coral Beach Hotel', 'Beautiful beachfront property with spa facilities', 'Mexico', 'Cozumel', 'Rafael E. Melgar 123', 4),
('Desert Oasis', 'Unique hotel in the heart of the desert', 'Mexico', 'San Luis Potosi', 'Carretera 57 Km 45', 3),
('Colonial Inn', 'Historic hotel in the city center', 'Mexico', 'Guanajuato', 'Calle Positos 45', 3),
('Barcelona Grand', 'Modern luxury hotel with city views', 'Spain', 'Barcelona', 'Passeig de Gràcia 68', 5),
('Madrid Central', 'Elegant hotel in the heart of Madrid', 'Spain', 'Madrid', 'Gran Vía 45', 4),
('Seville Palace', 'Traditional Andalusian architecture', 'Spain', 'Seville', 'Calle Sierpes 12', 4);

-- =====================================================
-- ROOMS
-- =====================================================

-- HOTEL 1: Ocean Paradise Resort
INSERT INTO Rooms (HotelId, Type, Description, PricePerNight, Photo) VALUES
(1, 'Oceanfront Presidential Suite', 'Luxurious suite with panoramic ocean views, private terrace, whirlpool tub, and butler service', 850.00, 'https://images.unsplash.com/photo-1582719478250-c89cae4dc85b'),
(1, 'Deluxe Ocean View Suite', 'Spacious suite with private balcony overlooking the Caribbean Sea', 450.00, 'https://images.unsplash.com/photo-1566665797739-1674de7a421a'),
(1, 'Premium Beachfront Room', 'Direct beach access, king-size bed, and stunning ocean views', 380.00, 'https://images.unsplash.com/photo-1578683010236-d716f9a3f461'),
(1, 'Standard Double Room', 'Comfortable room with garden view, queen bed, and modern amenities', 220.00, 'https://images.unsplash.com/photo-1568495248636-6432b97bd949'),
(1, 'Family Suite', 'Two-bedroom suite perfect for families, with separate living area', 520.00, 'https://images.unsplash.com/photo-1582719508461-905c673771fd'),
(1, 'Honeymoon Suite', 'Romantic suite with jacuzzi, ocean view, and champagne welcome', 620.00, 'https://images.unsplash.com/photo-1591088398332-8a7791972843'),
(1, 'Economy Room', 'Cozy room with all essentials, perfect for budget travelers', 150.00, 'https://images.unsplash.com/photo-1505693416388-ac5ce068fe85');

-- HOTEL 2: Mountain View Hotel
INSERT INTO Rooms (HotelId, Type, Description, PricePerNight, Photo) VALUES
(2, 'Alpine Suite', 'Luxury suite with floor-to-ceiling windows facing the Swiss Alps', 580.00, 'https://images.unsplash.com/photo-1520250497591-112f2f40a3f4'),
(2, 'Mountain View Double', 'Spacious room with balcony overlooking the majestic mountains', 320.00, 'https://images.unsplash.com/photo-1571008887538-b36bb32f4571'),
(2, 'Deluxe King Room', 'Elegant room with king bed, fireplace, and mountain views', 380.00, 'https://images.unsplash.com/photo-1590490360182-c33d57733427'),
(2, 'Standard Twin Room', 'Comfortable room with two twin beds, perfect for friends or colleagues', 250.00, 'https://images.unsplash.com/photo-1564013799919-ab600027ffc6'),
(2, 'Economy Single', 'Cozy single room with basic amenities, ideal for solo travelers', 150.00, 'https://images.unsplash.com/photo-1505693314120-0d443867891c'),
(2, 'Family Chalet', 'Two-level room with separate bedroom for kids, mountain views', 490.00, 'https://images.unsplash.com/photo-1571008887538-b36bb32f4571'),
(2, 'Penthouse Suite', 'Top-floor suite with panoramic views and private sauna', 720.00, 'https://images.unsplash.com/photo-1520250497591-112f2f40a3f4');

-- HOTEL 3: City Central Inn
INSERT INTO Rooms (HotelId, Type, Description, PricePerNight, Photo) VALUES
(3, 'Executive Suite', 'Modern suite with city views, separate workspace, and premium amenities', 420.00, 'https://images.unsplash.com/photo-1566665797739-1674de7a421a'),
(3, 'Deluxe Double Room', 'Elegant room with queen bed, city view, and luxury bathroom', 280.00, 'https://images.unsplash.com/photo-1568495248636-6432b97bd949'),
(3, 'City Studio', 'Modern studio with kitchenette, ideal for longer stays', 190.00, 'https://images.unsplash.com/photo-1505693314120-0d443867891c'),
(3, 'Standard Single', 'Comfortable single room with all essentials for business travelers', 120.00, 'https://images.unsplash.com/photo-1564013799919-ab600027ffc6'),
(3, 'Family Room', 'Spacious room with two double beds, perfect for families', 320.00, 'https://images.unsplash.com/photo-1582719508461-905c673771fd'),
(3, 'Premium King Room', 'Luxurious room with king bed, rain shower, and city skyline view', 350.00, 'https://images.unsplash.com/photo-1590490360182-c33d57733427');

-- HOTEL 4: Mayan Palace
INSERT INTO Rooms (HotelId, Type, Description, PricePerNight, Photo) VALUES
(4, 'Mayan Royal Suite', 'Luxury suite with traditional Mayan decor, private pool', 680.00, 'https://images.unsplash.com/photo-1582719478250-c89cae4dc85b'),
(4, 'Deluxe Garden View', 'Spacious room overlooking tropical gardens', 320.00, 'https://images.unsplash.com/photo-1578683010236-d716f9a3f461'),
(4, 'Standard Pool View', 'Comfortable room with pool access and modern amenities', 250.00, 'https://images.unsplash.com/photo-1568495248636-6432b97bd949');

-- HOTEL 5: Coral Beach Hotel
INSERT INTO Rooms (HotelId, Type, Description, PricePerNight, Photo) VALUES
(5, 'Oceanfront Villa', 'Private villa with direct beach access, infinity pool', 890.00, 'https://images.unsplash.com/photo-1582719478250-c89cae4dc85b'),
(5, 'Deluxe Beach Room', 'Elegant room with ocean views and private balcony', 420.00, 'https://images.unsplash.com/photo-1566665797739-1674de7a421a'),
(5, 'Coral Suite', 'Spacious suite with sea views and jacuzzi', 550.00, 'https://images.unsplash.com/photo-1578683010236-d716f9a3f461');

-- HOTEL 6: Desert Oasis
INSERT INTO Rooms (HotelId, Type, Description, PricePerNight, Photo) VALUES
(6, 'Desert View Suite', 'Unique suite with panoramic desert views', 380.00, 'https://images.unsplash.com/photo-1520250497591-112f2f40a3f4'),
(6, 'Standard Room', 'Comfortable room with modern amenities', 180.00, 'https://images.unsplash.com/photo-1505693314120-0d443867891c'),
(6, 'Oasis Double', 'Cozy room overlooking the hotel gardens', 220.00, 'https://images.unsplash.com/photo-1564013799919-ab600027ffc6');

-- HOTEL 7: Colonial Inn
INSERT INTO Rooms (HotelId, Type, Description, PricePerNight, Photo) VALUES
(7, 'Colonial Suite', 'Historic suite with original architecture and modern comforts', 320.00, 'https://images.unsplash.com/photo-1582719508461-905c673771fd'),
(7, 'Standard Double', 'Charming room with colonial-style decor', 160.00, 'https://images.unsplash.com/photo-1568495248636-6432b97bd949'),
(7, 'Courtyard Room', 'Peaceful room with views of the traditional courtyard', 190.00, 'https://images.unsplash.com/photo-1505693314120-0d443867891c');

-- HOTEL 8: Barcelona Grand
INSERT INTO Rooms (HotelId, Type, Description, PricePerNight, Photo) VALUES
(8, 'Grand Suite', 'Ultra-luxury suite with panoramic city views', 780.00, 'https://images.unsplash.com/photo-1582719478250-c89cae4dc85b'),
(8, 'Executive Room', 'Modern room with workspace and city view', 390.00, 'https://images.unsplash.com/photo-1590490360182-c33d57733427'),
(8, 'Deluxe Double', 'Elegant room with king bed and marble bathroom', 340.00, 'https://images.unsplash.com/photo-1566665797739-1674de7a421a'),
(8, 'Standard Twin', 'Comfortable room with two beds, ideal for groups', 260.00, 'https://images.unsplash.com/photo-1564013799919-ab600027ffc6');

-- HOTEL 9: Madrid Central
INSERT INTO Rooms (HotelId, Type, Description, PricePerNight, Photo) VALUES
(9, 'Royal Suite', 'Luxury suite with separate living area and city views', 620.00, 'https://images.unsplash.com/photo-1582719508461-905c673771fd'),
(9, 'Premium Double', 'Spacious room with elegant decor and modern amenities', 320.00, 'https://images.unsplash.com/photo-1578683010236-d716f9a3f461'),
(9, 'Standard Room', 'Comfortable accommodation in the heart of Madrid', 210.00, 'https://images.unsplash.com/photo-1568495248636-6432b97bd949');

-- HOTEL 10: Seville Palace
INSERT INTO Rooms (HotelId, Type, Description, PricePerNight, Photo) VALUES
(10, 'Palace Suite', 'Historic suite with traditional Andalusian architecture', 540.00, 'https://images.unsplash.com/photo-1520250497591-112f2f40a3f4'),
(10, 'Courtyard Double', 'Charming room overlooking the beautiful courtyard', 290.00, 'https://images.unsplash.com/photo-1590490360182-c33d57733427'),
(10, 'Standard Single', 'Cozy single room perfect for solo travelers', 140.00, 'https://images.unsplash.com/photo-1505693314120-0d443867891c');

-- =====================================================
-- BOOKINGS
-- =====================================================
INSERT INTO Bookings (RoomId, CustomerName, CustomerEmail, CustomerPhone, CheckInDate, CheckOutDate, Adults, Children, TotalPrice, Status) VALUES
(1, 'John Doe', 'john@example.com', '+1234567890', '2024-12-20', '2024-12-25', 2, 1, 1400.00, 'Confirmed'),
(5, 'Jane Smith', 'jane@example.com', '+0987654321', '2024-12-22', '2024-12-24', 2, 0, 240.00, 'Confirmed'),
(10, 'Maria Garcia', 'maria@example.com', '+34123456789', '2025-01-15', '2025-01-20', 2, 2, 1100.00, 'Confirmed'),
(15, 'Carlos Lopez', 'carlos@example.com', '+525512345678', '2025-02-10', '2025-02-15', 1, 0, 750.00, 'Pending');