namespace PremiumPlace.DTO.Bookings
{
    public record MyBookingDTO
    {
        public int Id { get; init; }
        public int PlaceId { get; init; }
        public string PlaceName { get; init; } = "";
        public DateOnly CheckInDate { get; init; }
        public DateOnly CheckOutDate { get; init; }
        public string Status { get; init; } = "";
        public DateTime CreatedAt { get; init; }
    }
}
