using System;

namespace PragueParkingV2_1.Domain
{
    internal class CheckoutResult
    {
        public string Identifier { get; }
        public DateTime ArrivalTime { get; }
        public DateTime DepartureTime { get; }
        public TimeSpan ParkingDuration { get; }
        public decimal HourlyPrice { get; }
        public int FreeMinutes { get; }
        public decimal TotalFee { get; }

        public CheckoutResult(
            string identifier,
            DateTime arrivalTime,
            DateTime departureTime,
            TimeSpan parkingDuration,
            decimal hourlyPrice,
            int freeMinutes,
            decimal totalFee)
        {
            Identifier = identifier;
            ArrivalTime = arrivalTime;
            DepartureTime = departureTime;
            ParkingDuration = parkingDuration;
            HourlyPrice = hourlyPrice;
            FreeMinutes = freeMinutes;
            TotalFee = totalFee;
        }
    }
}