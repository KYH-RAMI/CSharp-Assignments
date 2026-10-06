using System.Collections.Generic;

namespace PragueParkingV2.Domain
{
    internal class ParkingGarage
    {
        private const int MaxParkingSpots = 1_000;
        private readonly List<ParkingSpot> _parkingSpots = new();

        public IReadOnlyList<ParkingSpot> ParkingSpots => _parkingSpots;

        public ParkingGarage(int numberOfParkingSpots)
        {
            if (numberOfParkingSpots <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(numberOfParkingSpots),
                    "Number of parking spots must be greater than zero.");
            }

            if (numberOfParkingSpots > MaxParkingSpots)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(numberOfParkingSpots),
                    $"Number of parking spots cannot exceed {MaxParkingSpots}.");
            }
            for (int i = 1; i <= numberOfParkingSpots; i++)
            {
                _parkingSpots.Add(new ParkingSpot(i));
            }
        }

    }
}