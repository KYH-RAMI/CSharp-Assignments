using System;
using System.IO;
using System.Text.Json;

namespace PragueParkingV2_1.Data.Persistence
{
    public class ParkingDataService
    {
        private static string GetParkingDataPath()
        {
            return Path.Combine(
                AppContext.BaseDirectory,
                "Data",
                "parking-data.json");
        }

        public bool ParkingDataExists()
        {
            return File.Exists(GetParkingDataPath());
        }
        public ParkingDataDocument Load()
        {
            if (!ParkingDataExists())
            {
                return new ParkingDataDocument();
            }
            var json = File.ReadAllText(GetParkingDataPath());

            var parkingData =
                JsonSerializer.Deserialize<ParkingDataDocument>(json);

            if (parkingData is null)
            {
                throw new InvalidDataException(
                    "Parking data file could not be deserialized.");
            }

            ValidateParkingData(parkingData);

            return parkingData;
        }
        public void Save(ParkingDataDocument parkingData)
        {

            var path = GetParkingDataPath();

            var directory = Path.GetDirectoryName(path);

            if (directory is null)
            {
                throw new InvalidOperationException(
                    "Parking data directory could not be determined.");
            }

            Directory.CreateDirectory(directory);

            ValidateParkingData(parkingData);

            var jsonOptions = new JsonSerializerOptions
            {
                WriteIndented = true
            };
            var json = JsonSerializer.Serialize(parkingData, jsonOptions);
            var temporaryPath = path + ".tmp";

            try
            {
                File.WriteAllText(temporaryPath, json);
                if (!File.Exists(path))
                {
                    File.Move(temporaryPath, path);
                }
                else
                {
                    File.Replace(temporaryPath, path, null);
                }
            }

            finally
            {
                if (File.Exists(temporaryPath))
                {
                    File.Delete(temporaryPath);
                }
            }


        }
        private static void ValidateParkingData(ParkingDataDocument parkingData)
        {
            ArgumentNullException.ThrowIfNull(parkingData);

            if (parkingData.ParkingSpots is null)
            {
                throw new InvalidDataException(
                    "Parking data file is missing parking spots.");
            }

            var parkingSpotNumbers = new HashSet<int>();

            var vehicleIdentifiers = new HashSet<string>(
                StringComparer.OrdinalIgnoreCase);

            if (parkingData.ParkingSpots is null)
            {
                throw new InvalidDataException(
                    "Parking data file is missing parking spots.");
            }
            foreach (var spot in parkingData.ParkingSpots)
            {
                if (spot is null)
                {
                    throw new InvalidDataException(
                        "Parking data contains a null parking spot.");
                }

                if (spot.SpotNumber <= 0)
                {
                    throw new InvalidDataException(
                        "Parking spot number must be greater than zero.");
                }
                if (!parkingSpotNumbers.Add(spot.SpotNumber))
                {
                    throw new InvalidDataException(
                        $"Duplicate parking spot number: {spot.SpotNumber}");
                }
                if (spot.Vehicles is null)
                {
                    throw new InvalidDataException(
                        "Parking spot is missing vehicles.");
                }
                if (spot.Vehicles.Count == 0)
                {
                    throw new InvalidDataException(
                        "A saved parking spot must contain at least one vehicle.");
                }
                foreach (var vehicle in spot.Vehicles)
                {
                    if (vehicle is null)
                    {
                        throw new InvalidDataException(
                            "Parking data contains a null vehicle.");
                    }
                    if (string.IsNullOrWhiteSpace(vehicle.Identifier))
                    {
                        throw new InvalidDataException(
                            "Vehicle identifier cannot be empty.");
                    }
                    if (!vehicleIdentifiers.Add(vehicle.Identifier))
                    {
                        throw new InvalidDataException(
                            $"Duplicate vehicle identifier: {vehicle.Identifier}");
                    }
                    if (string.IsNullOrWhiteSpace(vehicle.VehicleType))
                    {
                        throw new InvalidDataException(
                            "Vehicle type is missing.");
                    }
                    if (vehicle.VehicleType != "Car" &&
                        vehicle.VehicleType != "MC" &&
                        vehicle.VehicleType != "Bicycle" &&
                        vehicle.VehicleType != "Bus")
                    {
                        throw new InvalidDataException(
                            $"Unsupported vehicle type: {vehicle.VehicleType}");
                    }
                    if (vehicle.ArrivalTime == DateTime.MinValue)
                    {
                        throw new InvalidDataException(
                            "Vehicle arrival time is invalid.");
                    }
                    if (vehicle.ArrivalTime == DateTime.MinValue)
                    {
                        throw new InvalidDataException(
                            "Vehicle arrival time is invalid.");
                    }
                    if (vehicle.ArrivalTime.Kind != DateTimeKind.Utc)
                    {
                        throw new InvalidDataException(
                            "Vehicle arrival time must be in UTC.");
                    }
                }

            }
        }



    }
}

