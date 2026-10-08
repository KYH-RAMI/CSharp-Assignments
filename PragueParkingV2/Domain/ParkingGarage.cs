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

        public ParkingSpot? FindAvailableSpot(
            Vehicle vehicle,
            int maxVehiclesPerSpot)
        {
            foreach (var parkingSpot in _parkingSpots)
            {
                if (parkingSpot.CanPark(vehicle, maxVehiclesPerSpot))
                {
                    return parkingSpot;
                }
            }

            return null;
        }

        public ParkingSpot ParkVehicle(
            Vehicle vehicle,
            int maxVehiclesPerSpot)
        {
            ArgumentNullException.ThrowIfNull(vehicle);

            if (FindVehicle(vehicle.Identifier) is not null)
            {
                throw new InvalidOperationException(
                    $"Vehicle '{vehicle.Identifier}' is already parked.");
            }

            var parkingSpot = FindAvailableSpot(
                vehicle,
                maxVehiclesPerSpot);

            if (parkingSpot is null)
            {
                throw new InvalidOperationException(
                    "No available parking spots.");
            }

            parkingSpot.ParkVehicle(
                vehicle,
                maxVehiclesPerSpot);

            return parkingSpot;
        }

        public Vehicle? FindVehicle(string identifier)
        {
            foreach (var spot in _parkingSpots)
            {
                var vehicle = spot.FindVehicle(identifier);
                if (vehicle is not null)
                {
                    return vehicle;
                }
            }

            return null;
        }

        public Vehicle? RemoveVehicle(string identifier)
        {
            foreach (var spot in _parkingSpots)
            {
                var vehicle = spot.RemoveVehicle(identifier);
                if (vehicle is not null)
                {
                    return vehicle;
                }
            }
            return null;
        }

    }
}