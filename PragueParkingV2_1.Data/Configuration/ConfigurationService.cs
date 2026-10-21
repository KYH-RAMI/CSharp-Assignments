using System;
using System.IO;
using System.Text.Json;
using System.Collections.Generic;


namespace PragueParkingV2_1.Data.Configuration
{
    public class ConfigurationService
    {
        private static string GetConfigurationPath()
        {
            return Path.Combine(
                AppContext.BaseDirectory,
                "Config",
                "config.json");
        }
        private static string GetTemplatePath()
        {
            return Path.Combine(
                AppContext.BaseDirectory,
                "Config",
                "config.template.json");
        }
        public AppConfiguration LoadConfiguration()
        {
            var path = GetConfigurationPath();

            /* Debugging load file
            Console.WriteLine($"Loading config from: {path}");
            Console.ReadKey(); */

            if (!File.Exists(path))
            {
                var templatePath = GetTemplatePath();

                if (!File.Exists(templatePath))
                {
                    throw new FileNotFoundException(
                        "Configuration template was not found.",
                        templatePath);
                }

                var directory = Path.GetDirectoryName(path);

                if (directory is null)
                {
                    throw new InvalidOperationException(
                        "Configuration directory could not be determined.");
                }

                Directory.CreateDirectory(directory);

                File.Copy(templatePath, path);
            }

            var json = File.ReadAllText(path);

            var configuration =
                JsonSerializer.Deserialize<AppConfiguration>(json);

            if (configuration is null)
            {
                throw new InvalidDataException(
                    "Configuration file could not be deserialized.");
            }

            ValidateConfiguration(configuration);

            return configuration;
        }

        public PricingConfiguration LoadPricing()
        {
            var path = GetConfigurationPath();

            if (!File.Exists(path))
            {
                throw new FileNotFoundException(
                    "Configuration file was not found.",
                    path);
            }

            var configuration = LoadConfiguration();

            return configuration.Pricing!;
        }


        private static void ValidateConfiguration(AppConfiguration configuration)
        {
            if (configuration.Pricing is null)
            {
                throw new InvalidDataException(
                    "Pricing configuration is missing.");
            }

            if (configuration.Garage is null)
            {
                throw new InvalidDataException(
                    "Garage configuration is missing.");
            }

            if (configuration.System is null)
            {
                throw new InvalidDataException(
                    "System configuration is missing.");
            }

            if (configuration.VehicleTypes is null)
            {
                throw new InvalidDataException(
                    "VehicleTypes configuration is missing.");
            }
            if (configuration.Garage.NumberOfParkingSpots <= 0)
            {
                throw new InvalidDataException(
                    "NumberOfParkingSpots must be greater than zero.");
            }

            if (configuration.Garage.NumberOfParkingSpots > 1_000)
            {
                throw new InvalidDataException(
                    "NumberOfParkingSpots cannot exceed 1,000.");
            }
            if (configuration.Garage.SpotCapacity <= 0)
            {
                throw new InvalidDataException(
                    "SpotCapacity must be greater than zero.");
            }

            if (configuration.Pricing.FreeMinutes < 0)
            {
                throw new InvalidDataException(
                    "FreeMinutes cannot be negative.");
            }

            if (configuration.Pricing.CarHourlyPrice < 0)
            {
                throw new InvalidDataException(
                    "CarHourlyPrice cannot be negative.");
            }

            if (configuration.Pricing.MotorcycleHourlyPrice < 0)
            {
                throw new InvalidDataException(
                    "MotorcycleHourlyPrice cannot be negative.");
            }
            if (configuration.Pricing.BicycleHourlyPrice < 0)
            {
                throw new InvalidDataException(
                    "BicycleHourlyPrice cannot be negative.");
            }

            if (configuration.Pricing.BusHourlyPrice < 0)
            {
                throw new InvalidDataException(
                    "BusHourlyPrice cannot be negative.");
            }

            if (string.IsNullOrWhiteSpace(configuration.System.TimeZone))
            {
                throw new InvalidDataException(
                    "TimeZone cannot be empty.");
            }

            try
            {
                TimeZoneInfo.FindSystemTimeZoneById(
                    configuration.System.TimeZone);
            }
            catch (TimeZoneNotFoundException ex)
            {
                throw new InvalidDataException(
                    $"Unknown time zone: {configuration.System.TimeZone}",
                    ex);
            }
            catch (InvalidTimeZoneException ex)
            {
                throw new InvalidDataException(
                    $"Invalid time zone: {configuration.System.TimeZone}",
                    ex);
            }

            if (configuration.VehicleTypes.Count == 0)
            {
                throw new InvalidDataException(
                    "At least one vehicle type must be configured.");
            }

            foreach (var (vehicleType, settings) in configuration.VehicleTypes)
            {
                if (string.IsNullOrWhiteSpace(vehicleType))
                {
                    throw new InvalidDataException(
                        "Vehicle type name cannot be empty.");
                }

                if (settings is null || settings.Size <= 0)
                {
                    throw new InvalidDataException(
                        $"Vehicle type '{vehicleType}' must have a size greater than zero.");
                }
            }
            string[] requiredVehicleTypes =
            {
                "Car",
                "MC",
                "Bicycle",
                "Bus"
            };

            foreach (string vehicleType in requiredVehicleTypes)
            {
                if (!configuration.VehicleTypes.ContainsKey(vehicleType))
                {
                    throw new InvalidDataException(
                        $"Required vehicle type '{vehicleType}' is missing.");
                }
            }
        }
    }

}
