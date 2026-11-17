namespace PragueParkingV2_1.Domain
{
    internal interface IParkingSpot
    {
        int SpotNumber
        {
            get;
        }

        IReadOnlyList<Vehicle> ParkedVehicles
        {
            get;
        }

        int GetUsedCapacity(Func<IVehicle, int> getSize);

        bool CanPark(
            IVehicle vehicle,
            int spotCapacity,
            Func<IVehicle, int> getSize);

        void ParkVehicle(
            Vehicle vehicle,
            int spotCapacity,
            Func<IVehicle, int> getSize);

        Vehicle? FindVehicle(string identifier);

        Vehicle? RemoveVehicle(string identifier);
    }
}