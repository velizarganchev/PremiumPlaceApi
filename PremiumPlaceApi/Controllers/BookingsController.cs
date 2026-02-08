using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PremiumPlace.DTO.Bookings;
using PremiumPlace_API.Controllers.Extensions;
using PremiumPlace_API.Services.Bookings;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace PremiumPlace_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingsController : ControllerBase
    {
        private readonly IBookingService _bookingService;

        public BookingsController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        // GET: /api/bookings/availability?placeId=1&from=2026-02-01&to=2026-03-31
        [HttpGet("availability")]
        public async Task<IActionResult> GetAvailability(
            [FromQuery] int placeId,
            [FromQuery] DateOnly from,
            [FromQuery] DateOnly to)
        {
            var sr = await _bookingService.GetAvailabilityAsync(placeId, from, to);
            return this.ToActionResult(sr);
        }

        // POST: /api/bookings
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateBooking([FromBody] CreateBookingRequest req)
        {
            var userId = GetUserIdOrNull();
            if (userId is null) return Unauthorized();

            var sr = await _bookingService.CreateBookingAsync(userId.Value, req);
            return this.ToActionResult(sr);
        }

        // GET: /api/bookings/my
        [Authorize]
        [HttpGet("my")]
        public async Task<IActionResult> GetMyBookings()
        {
            var userId = GetUserIdOrNull();
            if (userId is null) return Unauthorized();

            var sr = await _bookingService.GetMyBookingsAsync(userId.Value);
            return this.ToActionResult(sr);
        }

        // POST: /api/bookings/{id}/cancel
        [Authorize]
        [HttpPost("{id:int}/cancel")]
        public async Task<IActionResult> Cancel(int id)
        {
            var userId = GetUserIdOrNull();
            if (userId is null) return Unauthorized();

            var sr = await _bookingService.CancelBookingAsync(userId.Value, id);
            return this.ToActionResult(sr);
        }

        private int? GetUserIdOrNull()
        {
            var userIdStr =
                User.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? User.FindFirstValue(JwtRegisteredClaimNames.Sub);

            return int.TryParse(userIdStr, out var id) ? id : null;
        }
    }
}
