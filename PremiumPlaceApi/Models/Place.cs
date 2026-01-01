using System.ComponentModel.DataAnnotations;

namespace PremiumPlace_API.Models
{
    public class Place
    {
        public Place()
        {
            Amenitys = new HashSet<Amenity>();
            Reviews = new HashSet<Review>();
        }

        public int Id { get; set; }

        [Required, MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string? Details { get; set; }

        [Range(1, 30)]
        public int GuestCapacity { get; set; }

        [Range(0, 10000)]
        public decimal Rate { get; set; }

        [Range(0, 20)]
        public int Beds { get; set; }

        [Range(0, 23)]
        public int CheckInHour { get; set; }

        [Range(0, 23)]
        public int CheckOutHour { get; set; }

        [Range(10, 10000)]
        public int SquareFeet { get; set; }

        [MaxLength(500)]
        public string? ImageUrl { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public PlaceFeatures Features { get; set; } = new();
        public int CityId { get; set; }
        public City City { get; set; } = default!;
        public ICollection<Amenity> Amenitys { get; set; }
        public ICollection<Review> Reviews { get; set; }
    }
}
