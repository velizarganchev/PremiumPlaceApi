namespace PremiumPlace.DTO.Bookings
{
    public record ConfirmBookingRequest
    {
        public int BookingId { get; init; }
        public string PaymentReference { get; init; } = ""; // PayPal orderId/token later
    }
}
