using System;

namespace PragueParkingV2.Domain
{
    internal class ParkingFeeCalculator
    {
        private decimal _carHourlyPrice;
        private decimal _motorcycleHourlyPrice;
        private int _freeMinutes;

        public ParkingFeeCalculator(
            decimal carHourlyPrice, 
            decimal motorcycleHourlyPrice, 
            int freeMinutes)
        {
            UpdatePricing(
                carHourlyPrice,
                motorcycleHourlyPrice,
                freeMinutes);
          
        }

        public void UpdatePricing(
            decimal carHourlyPrice,
            decimal motorcycleHourlyPrice,
            int freeMinutes)
        {

            if (carHourlyPrice < 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(carHourlyPrice),
                    "Car hourly price cannot be negative.");
            }

            if(motorcycleHourlyPrice < 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(motorcycleHourlyPrice),
                    "Motorcycle hourly price cannot be negative.");
            }

            if (freeMinutes < 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(freeMinutes),
                    "Free minutes cannot be negative.");
            }

            _carHourlyPrice = carHourlyPrice;
            _motorcycleHourlyPrice = motorcycleHourlyPrice;
            _freeMinutes = freeMinutes;

        }
        public decimal CalculateParkingFee(
            Vehicle vehicle,
            DateTime departureTime)
        {
            // Validate input parameters
            ArgumentNullException.ThrowIfNull(vehicle);

            decimal hourlyPrice = vehicle switch
            {
                Car => _carHourlyPrice,
                MC => _motorcycleHourlyPrice,
                _ => throw new NotSupportedException(
                    $"Vehicle type '{vehicle.GetType().Name}' is not supported.")
            };

            if (departureTime.Kind != DateTimeKind.Utc)
            {
                throw new ArgumentException(
                    "Departure time must be in UTC.",
                    nameof(departureTime));
            }

            if (departureTime < vehicle.ArrivalTime)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(departureTime),
                    "Departure time cannot be before arrival time.");
            }

            // Calculate the parking duration
            TimeSpan duration =
                departureTime - vehicle.ArrivalTime;

            if (duration.TotalMinutes < _freeMinutes)
            {
                return 0;
            }

            // Calculate the number of started hours
            int startedHours =
                (int)Math.Ceiling(duration.TotalHours);

            decimal parkingFee =
                hourlyPrice * startedHours;

            return parkingFee;
        }
    }
}