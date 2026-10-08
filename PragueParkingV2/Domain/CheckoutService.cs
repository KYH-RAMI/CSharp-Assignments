using System;

namespace PragueParkingV2.Domain
{
    internal class CheckoutService
    {
        private readonly ParkingGarage _parkingGarage;
        private readonly ParkingFeeCalculator _feeCalculator;

        public CheckoutService(ParkingGarage parkingGarage, ParkingFeeCalculator feeCalculator)
        {
            ArgumentNullException.ThrowIfNull(parkingGarage, nameof(parkingGarage));
            ArgumentNullException.ThrowIfNull(feeCalculator, nameof(feeCalculator));

            _parkingGarage = parkingGarage;
            _feeCalculator = feeCalculator;
        }
    }
}
