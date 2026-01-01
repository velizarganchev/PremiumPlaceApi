using System.ComponentModel.DataAnnotations;

namespace PremiumPlace.DTO
{
    public record PlacePatchUpdateDTO
    {
        [MaxLength(200)]
        public string? Name { get; init; }

        [MaxLength(1000)]
        public string? Details { get; init; }

        [Range(1, 30)]
        public int? GuestCapacity { get; init; }

        [Range(0, 10000)]
        public decimal? Rate { get; init; }

        [Range(0, 20)]
        public int? Beds { get; init; }

        [Range(0, 23)]
        public int? CheckInHour { get; init; }

        [Range(0, 23)]
        public int? CheckOutHour { get; init; }

        [Range(10, 10000)]
        public int? SquareFeet { get; init; }

        [MaxLength(500)]
        public string? ImageUrl { get; init; }

        public int? CityId { get; init; }
        public PlaceFeaturesPatchDTO? Features { get; init; }
        public List<int>? AmenityIds { get; init; }
    }
}
