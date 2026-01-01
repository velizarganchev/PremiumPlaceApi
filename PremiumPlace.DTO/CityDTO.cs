using System;
using System.Collections.Generic;
using System.Text;

namespace PremiumPlace.DTO
{
    public record CityDTO
    {
        public int Id { get; init; }
        public required string Name { get; init; } = default!;
    }
}
