namespace PremiumPlace.DTO.Bookings
{
    public record DateRangeDto
    {
        public DateOnly From { get; init; }
        public DateOnly To { get; init; } // To is exclusive
    }
    public record AvailabilityResponse
    {
        public List<DateRangeDto> BlockedRanges { get; init; }

        public AvailabilityResponse(List<DateRangeDto> blockedRanges)
        {
            BlockedRanges = blockedRanges;
        }
    }
}
    