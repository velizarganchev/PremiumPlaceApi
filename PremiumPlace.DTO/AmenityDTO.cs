namespace PremiumPlace.DTO
{
    public record AmenityDTO
    {
        public int Id { get; set; }
        public required string Name { get; set; } = default!;
    }
}
