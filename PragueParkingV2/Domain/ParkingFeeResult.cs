using System;

namespace PragueParkingV2.Domain
{
    internal class ParkingFeeResult
    {
        public TimeSpan ParkingDuration { get; }
        public decimal HourlyPrice { get; }
        public int FreeMinutes { get; }
        public decimal TotalFee { get; }

        public ParkingFeeResult(
            TimeSpan parkingDuration,
            decimal hourlyPrice,
            int freeMinutes,
            decimal totalFee)
        {
            ParkingDuration = parkingDuration;
            HourlyPrice = hourlyPrice;
            FreeMinutes = freeMinutes;
            TotalFee = totalFee;
        }
    }
}