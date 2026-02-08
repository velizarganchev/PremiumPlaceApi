using PremiumPlace.DTO.Bookings;
using PremiumPlace_API.Models;
using PremiumPlace_API.Services;
using PremiumPlace_API.Services.Bookings;
using PremiumPlace_API.Tests.Helpers;
using Xunit;

namespace PremiumPlace_API.Tests;

public class BookingServiceTests : IDisposable
{
    private readonly TestDbContextFactory _factory;

    private const int PlaceId = 1;
    private const int User1Id = 1;
    private const int User2Id = 2;

    public BookingServiceTests()
    {
        _factory = new TestDbContextFactory();
        SeedBaseData();
    }

    private void SeedBaseData()
    {
        using var db = _factory.CreateContext();

        db.Cities.Add(new City { Id = 1, Name = "TestCity" });

        db.Places.Add(new Place
        {
            Id = PlaceId,
            Name = "Test Place",
            CityId = 1,
            GuestCapacity = 4,
            Rate = 100m,
            Beds = 2,
            SquareFeet = 500,
            CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
        });

        db.Users.AddRange(
            new User
            {
                Id = User1Id,
                Username = "user1",
                Email = "u1@test.com",
                PasswordHash = "hash1",
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new User
            {
                Id = User2Id,
                Username = "user2",
                Email = "u2@test.com",
                PasswordHash = "hash2",
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            }
        );

        db.SaveChanges();
    }

    // ───────────────────────── A ─────────────────────────
    [Fact]
    public async Task GetAvailability_PlaceIdZero_ReturnsFail()
    {
        // Arrange
        using var db = _factory.CreateContext();
        var svc = new BookingService(db);

        // Act
        var result = await svc.GetAvailabilityAsync(
            0,
            new DateOnly(2026, 2, 1),
            new DateOnly(2026, 2, 28));

        // Assert
        Assert.False(result.Success);
        Assert.Equal(ServiceErrorType.Validation, result.ErrorType);
    }

    // ───────────────────────── B ─────────────────────────
    [Fact]
    public async Task GetAvailability_ToBeforeOrEqualFrom_ReturnsFail()
    {
        using var db = _factory.CreateContext();
        var svc = new BookingService(db);

        var result = await svc.GetAvailabilityAsync(
            PlaceId,
            new DateOnly(2026, 3, 1),
            new DateOnly(2026, 2, 1));

        Assert.False(result.Success);
        Assert.Equal(ServiceErrorType.Validation, result.ErrorType);
    }

    // ───────────────────────── C ─────────────────────────
    [Fact]
    public async Task GetAvailability_NoOverlappingBookings_ReturnsEmptyBlockedList()
    {
        using var db = _factory.CreateContext();
        var svc = new BookingService(db);

        var result = await svc.GetAvailabilityAsync(
            PlaceId,
            new DateOnly(2026, 3, 1),
            new DateOnly(2026, 3, 31));

        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        Assert.Empty(result.Data.BlockedRanges);
    }

    // ───────────────────────── D ─────────────────────────
    [Fact]
    public async Task CreateBooking_CheckoutBeforeCheckin_ReturnsFail()
    {
        using var db = _factory.CreateContext();
        var svc = new BookingService(db);

        var req = new CreateBookingRequest
        {
            PlaceId = PlaceId,
            CheckInDate = new DateOnly(2026, 3, 10),
            CheckOutDate = new DateOnly(2026, 3, 5)
        };

        var result = await svc.CreateBookingAsync(User1Id, req);

        Assert.False(result.Success);
        Assert.Equal(ServiceErrorType.Validation, result.ErrorType);
    }

    // ───────────────────────── E ─────────────────────────
    [Fact]
    public async Task CreateBooking_DatesFree_Succeeds()
    {
        using var db = _factory.CreateContext();
        var svc = new BookingService(db);

        var req = new CreateBookingRequest
        {
            PlaceId = PlaceId,
            CheckInDate = new DateOnly(2026, 4, 1),
            CheckOutDate = new DateOnly(2026, 4, 5)
        };

        var result = await svc.CreateBookingAsync(User1Id, req);

        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        Assert.True(result.Data.BookingId > 0);

        // Verify persisted
        var booking = await db.Bookings.FindAsync(result.Data.BookingId);
        Assert.NotNull(booking);
        Assert.Equal(BookingStatus.Confirmed, booking.Status);
    }

    // ───────────────────────── F ─────────────────────────
    [Fact]
    public async Task CreateBooking_OverlapExists_ReturnsConflict()
    {
        using var db = _factory.CreateContext();

        // Seed existing booking: 2026-02-08 → 2026-02-10 (checkout exclusive)
        db.Bookings.Add(new Booking
        {
            PlaceId = PlaceId,
            UserId = User2Id,
            CheckInDate = new DateOnly(2026, 2, 8),
            CheckOutDate = new DateOnly(2026, 2, 10),
            Status = BookingStatus.Confirmed,
            CreatedAt = new DateTime(2026, 1, 15, 0, 0, 0, DateTimeKind.Utc)
        });
        db.SaveChanges();

        var svc = new BookingService(db);

        // Request overlapping: 2026-02-09 → 2026-02-11
        var req = new CreateBookingRequest
        {
            PlaceId = PlaceId,
            CheckInDate = new DateOnly(2026, 2, 9),
            CheckOutDate = new DateOnly(2026, 2, 11)
        };

        var result = await svc.CreateBookingAsync(User1Id, req);

        Assert.False(result.Success);
        Assert.Equal(ServiceErrorType.Conflict, result.ErrorType);
    }

    // ───────────────────────── G ─────────────────────────
    [Fact]
    public async Task CreateBooking_StartsOnExistingCheckout_Succeeds()
    {
        using var db = _factory.CreateContext();

        // Seed existing booking: 2026-02-08 → 2026-02-10 (checkout exclusive)
        db.Bookings.Add(new Booking
        {
            PlaceId = PlaceId,
            UserId = User2Id,
            CheckInDate = new DateOnly(2026, 2, 8),
            CheckOutDate = new DateOnly(2026, 2, 10),
            Status = BookingStatus.Confirmed,
            CreatedAt = new DateTime(2026, 1, 15, 0, 0, 0, DateTimeKind.Utc)
        });
        db.SaveChanges();

        var svc = new BookingService(db);

        // New booking starts exactly when existing ends → no overlap
        var req = new CreateBookingRequest
        {
            PlaceId = PlaceId,
            CheckInDate = new DateOnly(2026, 2, 10),
            CheckOutDate = new DateOnly(2026, 2, 12)
        };

        var result = await svc.CreateBookingAsync(User1Id, req);

        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        Assert.True(result.Data.BookingId > 0);
    }

    // ───────────────────────── H ─────────────────────────
    [Fact]
    public async Task GetMyBookings_ReturnsOnlyUserBookings_SortedByCreatedAtDesc()
    {
        using var db = _factory.CreateContext();

        db.Bookings.AddRange(
            new Booking
            {
                PlaceId = PlaceId,
                UserId = User1Id,
                CheckInDate = new DateOnly(2026, 5, 1),
                CheckOutDate = new DateOnly(2026, 5, 3),
                Status = BookingStatus.Confirmed,
                CreatedAt = new DateTime(2026, 1, 10, 0, 0, 0, DateTimeKind.Utc)
            },
            new Booking
            {
                PlaceId = PlaceId,
                UserId = User1Id,
                CheckInDate = new DateOnly(2026, 5, 5),
                CheckOutDate = new DateOnly(2026, 5, 7),
                Status = BookingStatus.Confirmed,
                CreatedAt = new DateTime(2026, 1, 20, 0, 0, 0, DateTimeKind.Utc)
            },
            new Booking
            {
                PlaceId = PlaceId,
                UserId = User2Id,
                CheckInDate = new DateOnly(2026, 6, 1),
                CheckOutDate = new DateOnly(2026, 6, 3),
                Status = BookingStatus.Confirmed,
                CreatedAt = new DateTime(2026, 1, 15, 0, 0, 0, DateTimeKind.Utc)
            }
        );
        db.SaveChanges();

        var svc = new BookingService(db);

        var result = await svc.GetMyBookingsAsync(User1Id);

        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        Assert.Equal(2, result.Data.Count);
        // All belong to User1
        Assert.All(result.Data, b => Assert.Equal(PlaceId, b.PlaceId));
        // Sorted desc by CreatedAt — newest first
        Assert.True(result.Data[0].CreatedAt > result.Data[1].CreatedAt);
        // Place name resolved via navigation
        Assert.Equal("Test Place", result.Data[0].PlaceName);
    }

    // ───────────────────────── I ─────────────────────────
    [Fact]
    public async Task CancelBooking_OwnBooking_Succeeds_AndIdempotent()
    {
        using var db = _factory.CreateContext();

        var booking = new Booking
        {
            PlaceId = PlaceId,
            UserId = User1Id,
            CheckInDate = new DateOnly(2026, 7, 1),
            CheckOutDate = new DateOnly(2026, 7, 3),
            Status = BookingStatus.Confirmed,
            CreatedAt = new DateTime(2026, 1, 10, 0, 0, 0, DateTimeKind.Utc)
        };
        db.Bookings.Add(booking);
        db.SaveChanges();

        var bookingId = booking.Id;
        var svc = new BookingService(db);

        // First cancel
        var result1 = await svc.CancelBookingAsync(User1Id, bookingId);
        Assert.True(result1.Success);

        // Verify status in DB
        var cancelled = await db.Bookings.FindAsync(bookingId);
        Assert.NotNull(cancelled);
        Assert.Equal(BookingStatus.Cancelled, cancelled.Status);

        // Cancel again — should still succeed (idempotent)
        var result2 = await svc.CancelBookingAsync(User1Id, bookingId);
        Assert.True(result2.Success);
        Assert.Contains("already cancelled", result2.Message!, StringComparison.OrdinalIgnoreCase);
    }

    public void Dispose()
    {
        _factory.Dispose();
    }
}
