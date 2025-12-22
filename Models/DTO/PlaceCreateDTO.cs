namespace PremiumPlace_API.Models.DTO
{
    public class PlaceCreateDTO
    {
        public required string Name { get; set; }
        public string? Details { get; set; }
        public decimal Rate { get; set; }
        public int SquareFeet { get; set; }
        public int Occupancy { get; set; }
        public string? ImageUrl { get; set; }
    }
}
