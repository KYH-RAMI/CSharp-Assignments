using PragueParkingV2_1.Data.Configuration;
using PragueParkingV2_1.Domain;

namespace PragueParkingV2_1.Application
{
    internal static class TestDataSeeder
    {
        public static ParkingGarage CreateGarage(
            AppConfiguration configuration)
        {
            ArgumentNullException.ThrowIfNull(configuration);

            var garageSettings = configuration.Garage!;

            var garage = new ParkingGarage(
                garageSettings.NumberOfParkingSpots);

            int spotCapacity = garageSettings.SpotCapacity;

            Func<IVehicle, int> getSize = vehicle =>
                configuration.VehicleTypes![vehicle.GetType().Name].Size;

            // Parkera bussen först så att fyra sammanhängande
            // lediga platser kan reserveras.
            var bus = new Bus("TESTBUS1");

            garage.ParkBus(
                bus,
                spotCapacity,
                getSize(bus),
                garageSettings.BusMaxParkingSpot);

            // Registrera övriga testfordon.
            garage.ParkVehicle(
                new Car("TESTCAR1"),
                spotCapacity,
                getSize);

            garage.ParkVehicle(
                new MC("TESTMC1"),
                spotCapacity,
                getSize);

            garage.ParkVehicle(
                new Bicycle("TESTBIKE1"),
                spotCapacity,
                getSize);

            return garage;
        }
    }
}