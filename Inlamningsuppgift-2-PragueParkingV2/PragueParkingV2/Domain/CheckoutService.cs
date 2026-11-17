using System;

namespace PragueParkingV2.Domain
{
    internal class CheckoutService
    {
        private readonly ParkingGarage _parkingGarage;
        private readonly ParkingFeeCalculator _feeCalculator;

        public CheckoutService(
            ParkingGarage parkingGarage,
            ParkingFeeCalculator feeCalculator)
        {
            ArgumentNullException.ThrowIfNull(parkingGarage);
            ArgumentNullException.ThrowIfNull(feeCalculator);

            _parkingGarage = parkingGarage;
            _feeCalculator = feeCalculator;
        }

        public CheckoutResult CheckOut(
            string identifier,
            DateTime departureTime)
        {
            if (string.IsNullOrWhiteSpace(identifier))
            {
                throw new ArgumentException(
                    "Identifier cannot be null, empty or whitespace.",
                    nameof(identifier));
            }

            identifier = identifier.Trim();

            var vehicle = _parkingGarage.FindVehicle(identifier);

            if (vehicle is null)
            {
                throw new InvalidOperationException(
                    $"Vehicle with identifier '{identifier}' was not found.");
            }

            var fee = _feeCalculator.CalculateParkingFee(
                vehicle,
                departureTime);

            var removedVehicle =
                _parkingGarage.RemoveVehicle(identifier);

            if (removedVehicle is null)
            {
                throw new InvalidOperationException(
                    $"Vehicle with identifier '{identifier}' could not be removed.");
            }

            return new CheckoutResult(
                removedVehicle.Identifier,
                removedVehicle.ArrivalTime,
                departureTime,
                fee.ParkingDuration,
                fee.HourlyPrice,
                fee.FreeMinutes,
                fee.TotalFee);
        }
    }
}
       