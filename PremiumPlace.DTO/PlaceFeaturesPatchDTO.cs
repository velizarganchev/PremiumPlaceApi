using System;
using System.Collections.Generic;
using System.Text;

namespace PremiumPlace.DTO
{
    public sealed record PlaceFeaturesPatchDTO
    {
        public bool? Internet { get; init; }
        public bool? AirConditioned { get; init; }
        public bool? PetsAllowed { get; init; }
        public bool? Parking { get; init; }
        public bool? Entertainment { get; init; }
        public bool? Kitchen { get; init; }
        public bool? Refrigerator { get; init; }
        public bool? Washer { get; init; }
        public bool? Dryer { get; init; }
        public bool? SelfCheckIn { get; init; }
    }
}
