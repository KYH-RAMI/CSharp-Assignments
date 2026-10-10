using System;
using System.Collections.Generic;
using System.Text;

namespace PragueParkingV2.Data.Persistence
{
    public class ParkingSpotData
    {
        public int SpotNumber { get; set; }
        public List<VehicleData> Vehicles { get; set; } = new();

    }
}
