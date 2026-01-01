using System.ComponentModel.DataAnnotations;

namespace PremiumPlace_API.Models
{
    public class City
    {
        public City()
        {
            Places = new HashSet<Place>();
        }

        [Key]
        public int Id { get; set; }
        [Required]
        public required string Name { get; set; }

        public virtual ICollection<Place> Places { get; set; }
    }
}
