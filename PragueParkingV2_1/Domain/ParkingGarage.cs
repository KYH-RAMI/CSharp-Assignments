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

        // Buss algorithm för reservera 4 parkeringsrutor
        public IReadOnlyList<ParkingSpot>? FindAvailableBusSpots(
            int busMaxParkingSpot,
            int requiredSpots)
        {
            if (busMaxParkingSpot < 1 ||
                busMaxParkingSpot > _parkingSpots.Count)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(busMaxParkingSpot));
            }

            if (requiredSpots <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(requiredSpots));
            }

            for (int startIndex = 0;
                 startIndex <= busMaxParkingSpot - requiredSpots;
                 startIndex++)
            {
                var availableSpots = new List<ParkingSpot>();

                for (int offset = 0; offset < requiredSpots; offset++)
                {
                    var spot = _parkingSpots[startIndex + offset];

                    if (spot.ParkedVehicles.Count > 0 ||
                        spot.ReservedByBus is not null)
                    {
                        break;
                    }

                    availableSpots.Add(spot);
                }

                if (availableSpots.Count == requiredSpots)
                {
                    return availableSpots;
                }
            }

            return null;
        }
        public IReadOnlyList<ParkingSpot>? FindAvailableBusSpotsAt(
            int startSpotNumber,
            int busMaxParkingSpot,
            int requiredSpots)
        {
            if (busMaxParkingSpot < 1 ||
                busMaxParkingSpot > _parkingSpots.Count)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(busMaxParkingSpot));
            }

            if (requiredSpots <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(requiredSpots));
            }

            if (startSpotNumber < 1 ||
                startSpotNumber > busMaxParkingSpot - requiredSpots + 1)
            {
                return null;
            }

            var availableSpots = new List<ParkingSpot>();

            for (int offset = 0; offset < requiredSpots; offset++)
            {
                var spot = _parkingSpots[startSpotNumber - 1 + offset];

                if (spot.ParkedVehicles.Count > 0 ||
                    spot.ReservedByBus is not null)
                {
                    return null;
                }

                availableSpots.Add(spot);
            }

            return availableSpots;
        }
        public ParkingSpot ParkBus(
            Bus bus,
            int spotCapacity,
            int busSize,
            int busMaxParkingSpot)
        {
            ArgumentNullException.ThrowIfNull(bus);

            const int requiredSpots = 4;

            if (spotCapacity <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(spotCapacity));
            }

            if (busSize != spotCapacity * requiredSpots)
            {
                throw new InvalidOperationException(
                    "Bus size must equal four parking spot capacities.");
            }

            if (FindVehicle(bus.Identifier) is not null)
            {
                throw new InvalidOperationException(
                    $"Vehicle '{bus.Identifier}' is already parked.");
            }

            var spots = FindAvailableBusSpots(
                busMaxParkingSpot,
                requiredSpots);

            if (spots is null)
            {
                throw new InvalidOperationException(
                    "No four consecutive parking spots are available.");
            }

            var reservedSpots = new List<ParkingSpot>();

            try
            {
                foreach (var spot in spots)
                {
                    spot.ReserveForBus(bus);
                    reservedSpots.Add(spot);
                }

                spots[0].ParkBusAtPrimarySpot(bus);

                return spots[0];
            }
            catch
            {
                spots[0].RemoveVehicle(bus.Identifier);

                foreach (var spot in reservedSpots)
                {
                    spot.ReleaseBusReservation(bus);
                }

                throw;
            }
        }
        public ParkingSpot ParkBusAtSpot(
            Bus bus,
            int startSpotNumber,
            int spotCapacity,
            int busSize,
            int busMaxParkingSpot)
        {
            ArgumentNullException.ThrowIfNull(bus);

            const int requiredSpots = 4;

            if (spotCapacity <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(spotCapacity));
            }

            if (busSize != spotCapacity * requiredSpots)
            {
                throw new InvalidOperationException(
                    "Bus size must equal four parking spot capacities.");
            }

            if (FindVehicle(bus.Identifier) is not null)
            {
                throw new InvalidOperationException(
                    $"Vehicle '{bus.Identifier}' is already parked.");
            }

            var spots = FindAvailableBusSpotsAt(
                startSpotNumber,
                busMaxParkingSpot,
                requiredSpots);

            if (spots is null)
            {
                throw new InvalidOperationException(
                    $"Cannot restore bus '{bus.Identifier}' " +
                    $"at parking spot {startSpotNumber:D3}.");
            }

            var reservedSpots = new List<ParkingSpot>();

            try
            {
                foreach (var spot in spots)
                {
                    spot.ReserveForBus(bus);
                    reservedSpots.Add(spot);
                }

                spots[0].ParkBusAtPrimarySpot(bus);

                return spots[0];
            }
            catch
            {
                spots[0].RemoveVehicle(bus.Identifier);

                foreach (var spot in reservedSpots)
                {
                    spot.ReleaseBusReservation(bus);
                }

                throw;
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
                Vehicle? vehicle = spot.FindVehicle(identifier);

                if (vehicle is null)
                {
                    continue;
                }

                if (vehicle is Bus bus)
                {
                    var reservedSpots = _parkingSpots
                        .Where(s => ReferenceEquals(s.ReservedByBus, bus))
                        .ToList();

                    if (reservedSpots.Count != 4 ||
                        !reservedSpots.Contains(spot))
                    {
                        throw new InvalidOperationException(
                            $"Bus '{identifier}' does not have four valid reservations.");
                    }

                    Vehicle? removedBus = spot.RemoveVehicle(identifier);

                    if (!ReferenceEquals(removedBus, bus))
                    {
                        throw new InvalidOperationException(
                            $"Bus '{identifier}' could not be removed.");
                    }

                    foreach (var reservedSpot in reservedSpots)
                    {
                        reservedSpot.ReleaseBusReservation(bus);
                    }

                    return bus;
                }

                return spot.RemoveVehicle(identifier);
            }

            return null;
        }
        public void MoveVehicle(
            string identifier,
            int targetSpotNumber,
            int spotCapacity,
            Func<IVehicle, int> getSize)
        {
            ArgumentNullException.ThrowIfNull(getSize);
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
            if (!targetSpot.CanPark(vehicle, spotCapacity, getSize))
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
                    spotCapacity,
                    getSize);
            }
            catch
            {
                sourceSpot.ParkVehicle(
                    removedVehicle,
                    spotCapacity,
                    getSize);

                throw;
            }


        }




    }
}