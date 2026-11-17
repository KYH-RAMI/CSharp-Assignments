namespace PragueParkingV2.Domain
{
    internal class ParkingSpot
    {
        public int SpotNumber { get; }

        private readonly List<Vehicle> _parkedVehicles = new();

        public IReadOnlyList<Vehicle> ParkedVehicles => _parkedVehicles;

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
        public bool CanPark(Vehicle vehicle, int maxVehiclesPerSpot)
        {
            ArgumentNullException.ThrowIfNull(vehicle);

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


        public void ParkVehicle(Vehicle vehicle, int maxVehiclesPerSpot)
        {
            if (!CanPark(vehicle, maxVehiclesPerSpot))
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