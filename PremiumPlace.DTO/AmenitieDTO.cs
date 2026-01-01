using System;
using System.Collections.Generic;
using System.Text;

namespace PremiumPlace.DTO
{
    public record AmenitieDTO
    {
        public int Id { get; set; }
        public required string Name { get; set; } = default!;
    }
}
