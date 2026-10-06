namespace PragueParkingV2.Domain
{
    internal class ParkingSpot
    {
        public int SpotNumber { get; }

        private readonly List<Vehicle> _parkedVehicles = new();

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
    }
}