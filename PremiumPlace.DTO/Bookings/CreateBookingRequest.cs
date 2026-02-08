namespace PremiumPlace.DTO.Bookings
{
    public record CreateBookingRequest
    {
        public int PlaceId { get; set; }
        public DateOnly CheckInDate { get; set; }
        public DateOnly CheckOutDate { get; set; }
    }
}
