

namespace PragueParkingV2_1.Domain
{
    internal class MC : Vehicle
    {
        public MC(string identifier) : base(identifier)
        {
        }
        public MC(string identifier, DateTime arrivalTime)
            : base(identifier, arrivalTime)
        {

        }
    }
}
