
namespace PragueParkingV2_1.Domain
{
    internal class Car : Vehicle
    {
        public Car(string identifier) : base(identifier)
        {
        }
        public Car(string identifier, DateTime arrivalTime)
            : base(identifier, arrivalTime)
        {

        }
    }
}
