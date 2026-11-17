using PragueParkingV2.Data.Persistence;
using PragueParkingV2.Domain;
using PragueParkingV2.Data.Configuration;

namespace PragueParkingV2.Application
{
    internal static class ParkingDataMapper
    {
        public static Vehicle ToVehicle(VehicleData data)
        {
            ArgumentNullException.ThrowIfNull(data);
            Vehicle vehicle = data.VehicleType switch
            {
                "Car" => new Car(data.Identifier, data.ArrivalTime),
                "MC" => new MC(data.Identifier, data.ArrivalTime),
                _ => throw new InvalidDataException(
                    $"Unsupported saved vehicle type: {data.VehicleType}.")
            };
            return vehicle;
        }

        public static VehicleData ToVehicleData(Vehicle vehicle)
        {
            ArgumentNullException.ThrowIfNull(vehicle);
            string vehicleType = vehicle switch
            {
                Car => "Car",
                MC => "MC",
                _ => throw new InvalidDataException(
                    $"Unsupported vehicle type: {vehicle.GetType().Name}.")
            };

            var data = new VehicleData
            {
                Identifier =  vehicle.Identifier,
                VehicleType = vehicleType,
                ArrivalTime = vehicle.ArrivalTime
            };

            return data;
        }

        public static ParkingDataDocument ToDocument(ParkingGarage garage)
        {
            ArgumentNullException.ThrowIfNull(garage);

            var document = new ParkingDataDocument();

            var occupiedSpots = garage.ParkingSpots
                .Where(spot => spot.ParkedVehicles.Count > 0);

            document.ParkingSpots = occupiedSpots
        .Select(spot => new ParkingSpotData
        {
            SpotNumber = spot.SpotNumber,
            Vehicles = spot.ParkedVehicles
            .Select(ToVehicleData)
            .ToList()
        })
        .ToList();

            return document;
        }

        public static ParkingGarage ToGarage(
            ParkingDataDocument document,
            AppConfiguration configuration)
        {
            ArgumentNullException.ThrowIfNull(document);
            ArgumentNullException.ThrowIfNull(configuration);

            var garage = new ParkingGarage(
                configuration.Garage!.NumberOfParkingSpots);

            foreach (var spotData in document.ParkingSpots)
            {
                if (spotData.SpotNumber < 1 ||
                    spotData.SpotNumber > garage.ParkingSpots.Count)
                {
                    throw new InvalidDataException(
                        $"Parking spot {spotData.SpotNumber} is outside " +
                        $"the configured garage capacity " +
                        $"(1-{garage.ParkingSpots.Count}).");
                }

                foreach (var vehicleData in spotData.Vehicles)
                {
                    Vehicle vehicle = ToVehicle(vehicleData);

                    int capacity = configuration.VehicleTypes![
                        vehicleData.VehicleType].VehiclesPerSpot;

                    try
                    {
                        garage.ParkVehicleAtSpot(
                            vehicle,
                            spotData.SpotNumber,
                            capacity);
                    }
                    catch (InvalidOperationException ex)
                    {
                        throw new InvalidDataException(
                            $"Cannot restore vehicle '{vehicle.Identifier}' " +
                            $"({vehicleData.VehicleType}) to parking spot " +
                            $"{spotData.SpotNumber:D3}: {ex.Message}",
                            ex);
                    }
                }
            }

            return garage;
        }
    }
}