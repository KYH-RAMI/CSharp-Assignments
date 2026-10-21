namespace PragueParkingV2_1.Domain
{
    internal class ParkingSpot : IParkingSpot
    {
        public int SpotNumber
        {
            get;
        }

        private readonly List<Vehicle> _parkedVehicles = new();

        public IReadOnlyList<Vehicle> ParkedVehicles => _parkedVehicles;
        public Bus? ReservedByBus
        {
            get;
            private set;
        }
        public void ReserveForBus(Bus bus)
        {
            ArgumentNullException.ThrowIfNull(bus);

            if (ReservedByBus is not null)
            {
                throw new InvalidOperationException(
                    $"Parking spot {SpotNumber:D3} is already reserved by a bus.");
            }

            if (_parkedVehicles.Count > 0)
            {
                throw new InvalidOperationException(
                    $"Parking spot {SpotNumber:D3} is occupied.");
            }

            ReservedByBus = bus;
        }
        public void ReleaseBusReservation(Bus bus)
        {
            ArgumentNullException.ThrowIfNull(bus);

            if (!ReferenceEquals(ReservedByBus, bus))
            {
                throw new InvalidOperationException(
                    $"Parking spot {SpotNumber:D3} is not reserved by this bus.");
            }

            ReservedByBus = null;
        }

        //Kontroll av att rätt buss reserverat P-platsen
        public void ParkBusAtPrimarySpot(Bus bus)
        {
            ArgumentNullException.ThrowIfNull(bus);

            if (!ReferenceEquals(ReservedByBus, bus))
            {
                throw new InvalidOperationException(
                    $"Parking spot {SpotNumber:D3} is not reserved by this bus.");
            }

            if (_parkedVehicles.Count > 0)
            {
                throw new InvalidOperationException(
                    $"Parking spot {SpotNumber:D3} is already occupied.");
            }

            _parkedVehicles.Add(bus);
        }
        public ParkingSpot(int spotNumber)
        {
            if (spotNumber <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(spotNumber),
                    "Parking spot number must be greater than zero.");
            }

            SpotNumber = spotNumber;
        }
        public int GetUsedCapacity(Func<IVehicle, int> getSize)
        {
            ArgumentNullException.ThrowIfNull(getSize);

            int usedCapacity = 0;

            foreach (var vehicle in _parkedVehicles)
            {
                int size = getSize(vehicle);

                if (size <= 0)
                {
                    throw new InvalidOperationException(
                        "Vehicle size must be greater than zero.");
                }

                usedCapacity += size;
            }

            return usedCapacity;
        }
        public bool CanPark(IVehicle vehicle, int maxVehiclesPerSpot)
        {
            ArgumentNullException.ThrowIfNull(vehicle);
            if (ReservedByBus is not null)
            {
                return false;
            }
            if (vehicle is Bus)
            {
                return false;
            }
            if (maxVehiclesPerSpot <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(maxVehiclesPerSpot),
                    "Maximum vehicles per spot must be greater than zero.");
            }

            if (_parkedVehicles.Count == 0)
            {
                return true;
            }

            if (vehicle.GetType() != _parkedVehicles[0].GetType())
            {
                return false;
            }

            return _parkedVehicles.Count < maxVehiclesPerSpot;
        }
        public bool CanPark(
            IVehicle vehicle,
            int spotCapacity,
            Func<IVehicle, int> getSize)
        {
            ArgumentNullException.ThrowIfNull(vehicle);
            ArgumentNullException.ThrowIfNull(getSize);
            if (ReservedByBus is not null)
            {
                return false;
            }
            if (vehicle is Bus)
            {
                return false;
            }
            if (spotCapacity <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(spotCapacity),
                    "Parking spot capacity must be greater than zero.");
            }

            int vehicleSize = getSize(vehicle);

            if (vehicleSize <= 0)
            {
                throw new InvalidOperationException(
                    "Vehicle size must be greater than zero.");
            }

            int usedCapacity = GetUsedCapacity(getSize);

            return vehicleSize <= spotCapacity - usedCapacity;
        }



        public void ParkVehicle(Vehicle vehicle, int maxVehiclesPerSpot)
        {
            if (!CanPark(vehicle, maxVehiclesPerSpot))
            {
                throw new InvalidOperationException(
                    "The vehicle cannot be parked in this parking spot.");
            }

            _parkedVehicles.Add(vehicle);
        }
        public void ParkVehicle(
            Vehicle vehicle, 
            int spotCapacity, 
            Func<IVehicle, int> getSize)
        {
            ArgumentNullException.ThrowIfNull(vehicle);

            if (!CanPark(vehicle, spotCapacity, getSize))
            {
                throw new InvalidOperationException(
                    "The vehicle cannot be parked in this parking spot.");
            }

            _parkedVehicles.Add(vehicle);
        }
        public Vehicle? FindVehicle(string identifier)
        {
            if (string.IsNullOrWhiteSpace(identifier))
            {
                throw new ArgumentException(
                    "Identifier cannot be null, empty or whitespace.",
                    nameof(identifier));
            }

            identifier = identifier.Trim();



            return _parkedVehicles.FirstOrDefault(
                vehicle => string.Equals(
                    vehicle.Identifier,
                    identifier,
                    StringComparison.OrdinalIgnoreCase));
        }

        public Vehicle? RemoveVehicle(string identifier)
        {
            var vehicle = FindVehicle(identifier);

            if (vehicle is null)
            {
                return null;
            }
            _parkedVehicles.Remove(vehicle);

            return vehicle;
        }

    }

}