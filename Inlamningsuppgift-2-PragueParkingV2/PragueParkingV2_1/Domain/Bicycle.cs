namespace PragueParkingV2_1.Domain
{
    internal class Bicycle : Vehicle
    {
        public Bicycle(string identifier) : base(identifier)
        {
        }
        public Bicycle(string identifier, DateTime arrivalTime)
            : base(identifier, arrivalTime)
        {

        }
    }
}
