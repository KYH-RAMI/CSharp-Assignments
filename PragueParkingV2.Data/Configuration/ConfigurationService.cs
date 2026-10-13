using System;
using System.IO;
using System.Text.Json;
using System.Collections.Generic;


namespace PragueParkingV2.Data.Configuration
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
        public AppConfiguration LoadConfiguration()
        {
            var path = GetConfigurationPath();

            /* Debugging load file
            Console.WriteLine($"Loading config from: {path}");
            Console.ReadKey(); */

            if (!File.Exists(path))
            {
                var defaultConfiguration = CreateDefaultConfiguration();

                ValidateConfiguration(defaultConfiguration);

                return defaultConfiguration;
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

        private static AppConfiguration CreateDefaultConfiguration()
        {
            var configuration = new AppConfiguration
            {
                Pricing = new PricingConfiguration
                {
                    FreeMinutes = 10,
                    CarHourlyPrice = 20,
                    MotorcycleHourlyPrice = 10
                },

                Garage = new GarageConfiguration
                {
                    NumberOfParkingSpots = 100
                },

                System = new SystemConfiguration
                {
                    TimeZone = "Europe/Prague"
                },

                VehicleTypes = new Dictionary<string, VehicleTypeConfiguration>
                {
                    ["Car"] = new VehicleTypeConfiguration
                    {
                        VehiclesPerSpot = 1
                    },

                    ["MC"] = new VehicleTypeConfiguration
                    {
                        VehiclesPerSpot = 2
                    }
                }
            };

            var path = GetConfigurationPath();

            var directory = Path.GetDirectoryName(path);

            if (directory is null)
            {
                throw new InvalidOperationException(
                    "Configuration directory could not be determined.");
            }

            Directory.CreateDirectory(directory);

            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };

            var json = JsonSerializer.Serialize(configuration, options);

            File.WriteAllText(path, json);

            return configuration;
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

            if (!configuration.VehicleTypes.TryGetValue
                ("Car", out var carConfiguration) || carConfiguration is null)
            {
                throw new InvalidDataException(
                    "VehicleTypes must contain a valid Car configuration.");
            }

            if (carConfiguration.VehiclesPerSpot <= 0)
            {
                throw new InvalidDataException(
                    "Car VehiclesPerSpot must be greater than zero.");
            }

            if (!configuration.VehicleTypes.TryGetValue
                ("MC", out var mcConfiguration) || mcConfiguration is null)
            {
                throw new InvalidDataException(
                    "VehicleTypes must contain a valid MC configuration.");
            }

            if (mcConfiguration.VehiclesPerSpot <= 0)
            {
                throw new InvalidDataException(
                    "MC VehiclesPerSpot must be greater than zero.");
            }
        }
    }

}
