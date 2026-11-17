using System;

namespace PragueParkingV2_1.Domain
{
    internal abstract class Vehicle : IVehicle
    {
        private const int MaxIdentifierLength = 30; // 30 characters max.

        public string Identifier { get; }
        public DateTime ArrivalTime { get; }

        protected Vehicle(string identifier) : 
            this(identifier, DateTime.UtcNow)
        {
        }
        protected Vehicle(string identifier, DateTime arrivalTime)
        {
            if (string.IsNullOrWhiteSpace(identifier))
            {
                throw new ArgumentException(
                    "Identifier cannot be null, empty or whitespace.",
                    nameof(identifier));
            }

            identifier = identifier.Trim();

            if (identifier.Length > MaxIdentifierLength)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(identifier),
                    $"Identifier cannot exceed {MaxIdentifierLength} characters.");
            }

            if (arrivalTime == default)
            {
                throw new ArgumentException(
                    "Arrival time must contain a valid value.",
                    nameof(arrivalTime));
            }

            if (arrivalTime.Kind != DateTimeKind.Utc)
            {
                throw new ArgumentException(
                    "Arrival time must be in UTC.",
                    nameof(arrivalTime));
            }

            Identifier = identifier;
            ArrivalTime = arrivalTime;
        }
    }
}