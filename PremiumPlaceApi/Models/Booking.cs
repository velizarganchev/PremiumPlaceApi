namespace PremiumPlace_API.Models
{
    public class Booking
    {
        public int Id { get; set; }

        public int PlaceId { get; set; }
        public Place Place { get; set; } = null!;

        public int UserId { get; set; }
        public User User { get; set; } = null!;

        // Check-in inclusive, check-out exclusive (standard booking model)
        public DateOnly CheckInDate { get; set; }
        public DateOnly CheckOutDate { get; set; }

        public BookingStatus Status { get; set; } = BookingStatus.Confirmed;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    public enum BookingStatus
    {
        Confirmed = 1,
        Cancelled = 2
    }
}
