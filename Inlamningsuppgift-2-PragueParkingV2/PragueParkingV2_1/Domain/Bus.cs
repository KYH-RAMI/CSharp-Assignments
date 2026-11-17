
namespace PragueParkingV2_1.Domain
{
    internal class Bus : Vehicle
    {
        public Bus(string identifier) : base(identifier)
        {
        }
        public Bus(string identifier, DateTime arrivalTime)
            : base(identifier, arrivalTime)
        {

        }
    }
}
