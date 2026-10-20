using System.Collections.Generic;

namespace PragueParkingV2_1.Domain
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
        public ParkingSpot ParkVehicle(
            Vehicle vehicle, 
            int spotCapacity, 
            Func<IVehicle, int> getSize)
        {
            ArgumentNullException.ThrowIfNull(vehicle);
            ArgumentNullException.ThrowIfNull(getSize);

            if (FindVehicle(vehicle.Identifier) is not null)
            {
                throw new InvalidOperationException(
                    $"Vehicle '{vehicle.Identifier}' is already parked.");
            }

            var parkingSpot = FindAvailableSpot(
                vehicle,
                spotCapacity,
                getSize);

            if (parkingSpot is null)
            {
                throw new InvalidOperationException(
                    "No available parking spots.");
            }

            parkingSpot.ParkVehicle(
                vehicle,
                spotCapacity,
                getSize);

            return parkingSpot;
        }

        public void ParkVehicleAtSpot(
            Vehicle vehicle,
            int spotNumber,
            int maxVehiclesPerSpot)
        {
            ArgumentNullException.ThrowIfNull(vehicle);
            var parkingSpot = _parkingSpots.Find(
                spot => spot.SpotNumber == spotNumber);
            if (FindVehicle(vehicle.Identifier) is not null)
            {
                throw new InvalidOperationException(
                    $"Vehicle '{vehicle.Identifier}' is already parked.");
            }
            if (parkingSpot is null)
            {
                throw new InvalidOperationException(
                    $"Parking spot {spotNumber} does not exist.");
            }

            parkingSpot.ParkVehicle(
                vehicle,
                maxVehiclesPerSpot);
        }
        public void ParkVehicleAtSpot(
            Vehicle vehicle,
            int spotNumber,
            int spotCapacity,
            Func<IVehicle, int> getSize)
        {
            ArgumentNullException.ThrowIfNull(vehicle);
            ArgumentNullException.ThrowIfNull(getSize);

            if (FindVehicle(vehicle.Identifier) is not null)
            {
                throw new InvalidOperationException(
                    $"Vehicle '{vehicle.Identifier}' is already parked.");
            }

            var parkingSpot = _parkingSpots.Find(
                spot => spot.SpotNumber == spotNumber);

            if (parkingSpot is null)
            {
                throw new InvalidOperationException(
                    $"Parking spot {spotNumber} does not exist.");
            }

            parkingSpot.ParkVehicle(
                vehicle,
                spotCapacity,
                getSize);
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
        public ParkingSpot? FindAvailableSpot(
            IVehicle vehicle,
            int spotCapacity,
            Func<IVehicle, int> getSize)
        {
            ArgumentNullException.ThrowIfNull(vehicle);
            ArgumentNullException.ThrowIfNull(getSize);

            foreach (var parkingSpot in _parkingSpots)
            {
                if (parkingSpot.CanPark(vehicle, spotCapacity, getSize))
                {
                    return parkingSpot;
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
        public void MoveVehicle(
            string identifier,
            int targetSpotNumber,
            int maxVehiclesPerSpot)
        {
            var vehicle = FindVehicle(identifier);

            if (vehicle is null)
            {
                throw new InvalidOperationException(
                    $"Vehicle '{identifier}' was not found.");
            }
            var sourceSpot = _parkingSpots.Find(
                spot => spot.ParkedVehicles.Contains(vehicle));

            if (sourceSpot is null)
            {
                throw new InvalidOperationException(
                    $"The parking spot for vehicle '{identifier}' was not found.");
            }

            var targetSpot = _parkingSpots.Find(
                spot => spot.SpotNumber == targetSpotNumber);
            if (targetSpot is null)
            {
                throw new InvalidOperationException(
                    $"Parking spot {targetSpotNumber} does not exist.");
            }
            if (sourceSpot.SpotNumber == targetSpot.SpotNumber)
            {
                throw new InvalidOperationException(
                    $"Vehicle '{identifier}' is already parked at spot {targetSpotNumber}.");
            }
            if (!targetSpot.CanPark(vehicle, maxVehiclesPerSpot))
            {
                throw new InvalidOperationException(
                    $"Vehicle cannot be parked at spot {targetSpotNumber}.");
            }

            var removedVehicle = sourceSpot.RemoveVehicle(identifier);

            if (removedVehicle is null)
            {
                throw new InvalidOperationException(
                    $"Vehicle '{identifier}' could not be removed.");
            }
            try
            {
                targetSpot.ParkVehicle(
                    removedVehicle,
                    maxVehiclesPerSpot);
            }
            catch
            {
                sourceSpot.ParkVehicle(
                    removedVehicle,
                    maxVehiclesPerSpot);

                throw;
            }


        }




    }
}