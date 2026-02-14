namespace PremiumPlace.DTO.Bookings
{
    public record ConfirmBookingResult
    {
        public int BookingId { get; init; }
        public string Status { get; init; } = "";
    }
}
