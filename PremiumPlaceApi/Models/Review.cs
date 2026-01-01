using System.ComponentModel.DataAnnotations;

namespace PremiumPlace_API.Models
{
    public class Review
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int PlaceId { get; set; }
        public Place Place { get; set; } = null!;
        [Required]
        public int UserId { get; set; }
        public User User { get; set; } = null!;
        [Range(1, 5)]
        public int Rating { get; set; }
        [MaxLength(1000)]
        public string Comment { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
