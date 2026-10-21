using PragueParkingV2_1.Data.Persistence;
using PragueParkingV2_1.Domain;
using PragueParkingV2_1.Data.Configuration;

namespace PragueParkingV2_1.Application
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
                "Bicycle" => new Bicycle(data.Identifier, data.ArrivalTime),
                "Bus" => new Bus(data.Identifier, data.ArrivalTime),
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
                Bicycle => "Bicycle",
                Bus => "Bus",
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

            int spotCapacity = configuration.Garage.SpotCapacity;

            Func<IVehicle, int> getSize = vehicle =>
                configuration.VehicleTypes![vehicle.GetType().Name].Size;

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


                    try
                    {
                        if (vehicle is Bus bus)
                        {
                            garage.ParkBusAtSpot(
                                bus,
                                spotData.SpotNumber,
                                spotCapacity,
                                getSize(bus),
                                configuration.Garage.BusMaxParkingSpot);
                        }
                        else
                        {
                            garage.ParkVehicleAtSpot(
                                vehicle,
                                spotData.SpotNumber,
                                spotCapacity,
                                getSize);
                        }
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