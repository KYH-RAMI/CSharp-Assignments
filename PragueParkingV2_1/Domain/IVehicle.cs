namespace PragueParkingV2_1.Domain
{
    internal interface IVehicle
    {
        string Identifier { get; }
        DateTime ArrivalTime { get; }
    }
}