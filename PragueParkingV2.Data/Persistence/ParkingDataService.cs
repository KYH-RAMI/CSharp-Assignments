using System;
using System.IO;
using System.Text.Json;

namespace PragueParkingV2.Data.Persistence
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

           var json = JsonSerializer.Serialize(parkingData);
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
                if (spot.Vehicles is null)
                {
                    throw new InvalidDataException(
                        "Parking spot is missing vehicles.");
                }

            }
    }


    }
}

