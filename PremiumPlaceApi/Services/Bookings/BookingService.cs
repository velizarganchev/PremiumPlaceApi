using Microsoft.EntityFrameworkCore;
using PremiumPlace.DTO.Bookings;
using PremiumPlace_API.Data;
using PremiumPlace_API.Models;

namespace PremiumPlace_API.Services.Bookings
{
    public class BookingService : IBookingService
    {
        private readonly ApplicationDbContext _db;

        public BookingService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<ServiceResponse<AvailabilityResponse>> GetAvailabilityAsync(int placeId, DateOnly from, DateOnly to)
        {
            if (placeId <= 0)
                return Fail<AvailabilityResponse>("placeId is required.", ServiceErrorType.Validation);

            if (to <= from)
                return Fail<AvailabilityResponse>("Invalid range: 'to' must be after 'from'.", ServiceErrorType.Validation);

            var blocked = await _db.Bookings
                .AsNoTracking()
                .Where(b => b.PlaceId == placeId && b.Status == BookingStatus.Confirmed)
                // overlap: existingStart < requestedEnd && existingEnd > requestedStart
                .Where(b => b.CheckInDate < to && b.CheckOutDate > from)
                .Select(b => new DateRangeDto
                {
                    From = b.CheckInDate,
                    To = b.CheckOutDate
                })
                .ToListAsync();

            return new ServiceResponse<AvailabilityResponse>
            {
                Success = true,
                Data = new AvailabilityResponse(blocked),
                Message = "Availability retrieved successfully."
            };
        }

        public async Task<ServiceResponse<CreateBookingResult>> CreateBookingAsync(int userId, CreateBookingRequest req)
        {
            if (userId <= 0)
                return Fail<CreateBookingResult>("Unauthorized.", ServiceErrorType.Unauthorized);

            if (req is null)
                return Fail<CreateBookingResult>("Booking data is required.", ServiceErrorType.Validation);

            if (req.PlaceId <= 0)
                return Fail<CreateBookingResult>("PlaceId is required.", ServiceErrorType.Validation);

            if (req.CheckOutDate <= req.CheckInDate)
                return Fail<CreateBookingResult>("Check-out must be after check-in.", ServiceErrorType.Validation);

            var nights = req.CheckOutDate.DayNumber - req.CheckInDate.DayNumber;
            if (nights < 1)
                return Fail<CreateBookingResult>("Minimum stay is 1 night.", ServiceErrorType.Validation);

            var placeExists = await _db.Places.AsNoTracking().AnyAsync(p => p.Id == req.PlaceId);
            if (!placeExists)
                return Fail<CreateBookingResult>("Place not found.", ServiceErrorType.NotFound);

            // final overlap check (server truth)
            var hasOverlap = await _db.Bookings
                .Where(b => b.PlaceId == req.PlaceId && b.Status == BookingStatus.Confirmed)
                .AnyAsync(b => req.CheckInDate < b.CheckOutDate && req.CheckOutDate > b.CheckInDate);

            if (hasOverlap)
                return Fail<CreateBookingResult>("These dates are no longer available.", ServiceErrorType.Conflict);

            var booking = new Booking
            {
                PlaceId = req.PlaceId,
                UserId = userId,
                CheckInDate = req.CheckInDate,
                CheckOutDate = req.CheckOutDate,
                Status = BookingStatus.Confirmed,
                CreatedAt = DateTime.UtcNow
            };

            await _db.Bookings.AddAsync(booking);
            await _db.SaveChangesAsync();

            return new ServiceResponse<CreateBookingResult>
            {
                Success = true,
                Data = new CreateBookingResult { BookingId = booking.Id }
                ,
                Message = "Booking created successfully."
            };
        }

        public async Task<ServiceResponse<List<MyBookingDTO>>> GetMyBookingsAsync(int userId)
        {
            if (userId <= 0)
                return Fail<List<MyBookingDTO>>("Unauthorized.", ServiceErrorType.Unauthorized);

            var items = await _db.Bookings
                .AsNoTracking()
                .Where(b => b.UserId == userId)
                .OrderByDescending(b => b.CreatedAt)
                .Select(b => new MyBookingDTO
                {
                    Id = b.Id,
                    PlaceId = b.PlaceId,
                    PlaceName = b.Place.Name,
                    CheckInDate = b.CheckInDate,
                    CheckOutDate = b.CheckOutDate,
                    Status = b.Status.ToString(),
                    CreatedAt = b.CreatedAt
                })
                .ToListAsync();

            return new ServiceResponse<List<MyBookingDTO>>
            {
                Success = true,
                Data = items,
                Message = "My bookings retrieved successfully."
            };
        }

        public async Task<ServiceResponse<object>> CancelBookingAsync(int userId, int bookingId)
        {
            if (userId <= 0)
                return Fail<object>("Unauthorized.", ServiceErrorType.Unauthorized);

            if (bookingId <= 0)
                return Fail<object>("Invalid booking ID.", ServiceErrorType.Validation);

            var booking = await _db.Bookings.FirstOrDefaultAsync(b => b.Id == bookingId && b.UserId == userId);
            if (booking is null)
                return Fail<object>("Booking not found.", ServiceErrorType.NotFound);

            if (booking.Status == BookingStatus.Cancelled)
                return new ServiceResponse<object> { Success = true, Message = "Booking already cancelled." };

            booking.Status = BookingStatus.Cancelled;
            await _db.SaveChangesAsync();

            return new ServiceResponse<object>
            {
                Success = true,
                Message = "Booking cancelled successfully."
            };
        }

        private static ServiceResponse<T> Fail<T>(string message, ServiceErrorType type)
            => new() { Success = false, Message = message, ErrorType = type };
    }
}
