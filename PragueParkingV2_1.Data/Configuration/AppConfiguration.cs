using System.Collections.Generic;

namespace PragueParkingV2_1.Data.Configuration
{
    public class AppConfiguration
    {
        public PricingConfiguration? Pricing { get; set; }
        public GarageConfiguration? Garage { get; set; }
        public SystemConfiguration? System { get; set; }
        public Dictionary<string, VehicleTypeConfiguration>? VehicleTypes { get; set; }
    }

    public class PricingConfiguration
    {
        public int FreeMinutes { get; set; }
        public decimal CarHourlyPrice { get; set; }
        public decimal MotorcycleHourlyPrice { get; set; }
    }

    public class GarageConfiguration
    {
        public int NumberOfParkingSpots { get; set; }
        public int SpotCapacity { get; set; }
    }

    public class SystemConfiguration
    {
        public string TimeZone { get; set; } = string.Empty;
    }

    public class VehicleTypeConfiguration
    {
        public int VehiclesPerSpot { get; set; }
        public int Size { get; set; }
    }
}