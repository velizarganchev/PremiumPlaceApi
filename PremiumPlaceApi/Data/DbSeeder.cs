using Microsoft.EntityFrameworkCore;
using PremiumPlace_API.Models;
using PremiumPlace_API.Services.Auth;

namespace PremiumPlace_API.Data
{
    public class DbSeeder
    {
        public static async Task SeedAsync(ApplicationDbContext db, IPasswordService passwordService)
        {

            // Prevent reseeding
            if (await db.Places.AnyAsync())
                return;

            // ---------- Cities ----------
            var cities = new List<City>
            {
                new City { Name = "Berlin" },
                new City { Name = "Munich" },
                new City { Name = "Hamburg" },
                new City { Name = "Frankfurt" },
                new City { Name = "Cologne" }
            };

            db.Cities.AddRange(cities);
            await db.SaveChangesAsync();

            // ---------- Amenities ----------
            var wifi = new Amenity { Name = "Free Wi-Fi" };
            var pool = new Amenity { Name = "Swimming Pool" };
            var gym = new Amenity { Name = "Gym Access" };
            var roomService = new Amenity { Name = "24/7 Room Service" };
            var shuttle = new Amenity { Name = "Airport Shuttle" };

            db.Amenitys.AddRange(wifi, pool, gym, roomService, shuttle);
            await db.SaveChangesAsync();

            // ---------- Users ----------
            // TODO: Replace with your real password hashing (PasswordHasher)
            var admin = new User
            {
                Username = "admin",
                Email = "admin@premiumplace.local",
                Role = UserRole.Admin,
                CreatedAt = DateTime.UtcNow
            };
            admin.PasswordHash = passwordService.Hash(admin, "AdminPassword123!");

            var user = new User
            {
                Username = "demo",
                Email = "demo@premiumplace.local",
                Role = UserRole.User,
                CreatedAt = DateTime.UtcNow
            };
            user.PasswordHash = passwordService.Hash(user, "User");

            db.Users.AddRange(admin, user);
            await db.SaveChangesAsync();

            // ---------- Places ----------
            var cityIndex = 0;

            var places = new List<Place>
            {
                new Place
                {
                    Name = "Premium Place A",
                    Details = "A luxurious place to stay.",
                    Rate = 250m,
                    SquareFeet = 1500,
                    ImageUrl = "https://dotnetmasteryimages.blob.core.windows.net/bluevillaimages/villa1.jpg",
                    CreatedAt = DateTime.UtcNow,
                    CityId = cities[cityIndex++ % cities.Count].Id,
                    Features = new PlaceFeatures { Internet = true, AirConditioned = true, Parking = true, Kitchen = true },
                    Amenitys = { wifi, pool, gym }
                },
                new Place
                {
                    Name = "Premium Place B",
                    Details = "An elegant and comfortable place.",
                    Rate = 300m,
                    SquareFeet = 1800,
                    ImageUrl = "https://dotnetmasteryimages.blob.core.windows.net/bluevillaimages/villa2.jpg",
                    CreatedAt = DateTime.UtcNow,
                    CityId = cities[cityIndex++ % cities.Count].Id,
                    Features = new PlaceFeatures { Internet = true, AirConditioned = true, PetsAllowed = true, Kitchen = true },
                    Amenitys = { wifi, roomService, shuttle }
                },
                new Place
                {
                    Name = "Premium Place C",
                    Details = "A modern place with all amenities.",
                    Rate = 200m,
                    SquareFeet = 1200,
                    ImageUrl = "https://dotnetmasteryimages.blob.core.windows.net/bluevillaimages/villa3.jpg",
                    CreatedAt = DateTime.UtcNow,
                    CityId = cities[cityIndex++ % cities.Count].Id,
                    Features = new PlaceFeatures { Internet = true, Entertainment = true, Kitchen = true },
                    Amenitys = { pool, gym, shuttle }
                },
                new Place
                {
                    Name = "Premium Place D",
                    Details = "Premium place with spa facilities and concierge services.",
                    Rate = 900m,
                    SquareFeet = 4000,
                    ImageUrl = "https://dotnetmasteryimages.blob.core.windows.net/bluevillaimages/villa4.jpg",
                    CreatedAt = DateTime.UtcNow,
                    CityId = cities[cityIndex++ % cities.Count].Id,
                    Features = new PlaceFeatures { Internet = true, AirConditioned = true, Parking = true, SelfCheckIn = true },
                    Amenitys = { wifi, pool, gym, roomService, shuttle }
                },
                new Place
                {
                    Name = "Premium Place E",
                    Details = "Elegant villa with marble interiors and panoramic mountain views.",
                    Rate = 750m,
                    SquareFeet = 3200,
                    ImageUrl = "https://dotnetmasteryimages.blob.core.windows.net/bluevillaimages/villa5.jpg",
                    CreatedAt = DateTime.UtcNow,
                    CityId = cities[cityIndex++ % cities.Count].Id,
                    Features = new PlaceFeatures { Internet = true, AirConditioned = true, Parking = true, Kitchen = true, Washer = true, Dryer = true },
                    Amenitys = { wifi, pool, gym, roomService }
                }
            };

            await db.Places.AddRangeAsync(places);
            await db.SaveChangesAsync();

            // ---------- Reviews ----------
            // Unique index (PlaceId, UserId) => one review per user per place.
            var reviews = new List<Review>();

            foreach (var place in places)
            {
                reviews.Add(new Review
                {
                    PlaceId = place.Id,
                    UserId = admin.Id,
                    Rating = 5,
                    Comment = "Top quality! Everything was perfect.",
                    CreatedAt = DateTime.UtcNow.AddDays(-2)
                });

                reviews.Add(new Review
                {
                    PlaceId = place.Id,
                    UserId = user.Id,
                    Rating = 4,
                    Comment = "Great stay, very comfortable. Would visit again.",
                    CreatedAt = DateTime.UtcNow.AddDays(-1)
                });
            }

            db.Reviews.AddRange(reviews);
            await db.SaveChangesAsync();
        }
    }
}
