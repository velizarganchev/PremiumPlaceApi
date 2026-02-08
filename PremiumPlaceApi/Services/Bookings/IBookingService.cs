using PremiumPlace.DTO.Bookings;

namespace PremiumPlace_API.Services.Bookings
{
    public interface IBookingService
    {
        Task<ServiceResponse<AvailabilityResponse>> GetAvailabilityAsync(int placeId, DateOnly from, DateOnly to);

        Task<ServiceResponse<CreateBookingResult>> CreateBookingAsync(int userId, CreateBookingRequest req);

        Task<ServiceResponse<List<MyBookingDTO>>> GetMyBookingsAsync(int userId);

        Task<ServiceResponse<object>> CancelBookingAsync(int userId, int bookingId);
    }
}
