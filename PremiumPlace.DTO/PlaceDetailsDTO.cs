namespace PremiumPlace.DTO
{
    public sealed record PlaceDetailsDTO : PlaceDTO
    {
        public ReviewSummaryDTO ReviewSummary { get; set; } = new();
        public List<ReviewDTO> Reviews { get; set; } = new();
    }
}
