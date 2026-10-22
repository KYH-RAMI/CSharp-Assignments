using PragueParkingV2_1.Data.Configuration;
using PragueParkingV2_1.Domain;

namespace PragueParkingV2_1.Application
{
    internal sealed record DemoStep(
        string Description,
        int FocusSpot,
        Action Execute,
        int DelayMs = 150);

    internal static class DemoScenario
    {
        public const int DemoParkingSpots = 100;

        private sealed record DemoOperation(int Spot, Action Execute);

        public static IReadOnlyList<DemoStep> Create(
            ParkingGarage garage,
            AppConfiguration configuration)
        {
            ArgumentNullException.ThrowIfNull(garage);
            ArgumentNullException.ThrowIfNull(configuration);

            if (garage.ParkingSpots.Count != DemoParkingSpots)
            {
                throw new InvalidOperationException(
                    "Live Demonstration requires a separate 100-spot garage.");
            }

            var settings = configuration.Garage!;
            int spotCapacity = settings.SpotCapacity;
            int busLimit = Math.Min(settings.BusMaxParkingSpot, DemoParkingSpots);

            // This scripted example relies on standard size proportions.
            // Normal parking operations are not affected by these demo limits.
            if (spotCapacity != 4 ||
                configuration.VehicleTypes!["Car"].Size != 4 ||
                configuration.VehicleTypes["MC"].Size != 2 ||
                configuration.VehicleTypes["Bicycle"].Size != 1 ||
                configuration.VehicleTypes["Bus"].Size != 16 ||
                busLimit < 9)
            {
                throw new InvalidOperationException(
                    "Live Demonstration uses the standard capacities " +
                    "(spot 4, Car 4, MC 2, Bicycle 1, Bus 16) " +
                    "and requires at least 9 bus-eligible spots.");
            }

            Func<IVehicle, int> getSize = vehicle =>
                configuration.VehicleTypes![vehicle.GetType().Name].Size;

            var steps = new List<DemoStep>();
            var arrivals = new List<DemoOperation>();
            var completions = new List<DemoOperation>();
            var parkedVehicles = new List<(int Spot, string Identifier)>();

            void Park(Vehicle vehicle, int spot)
            {
                garage.ParkVehicleAtSpot(vehicle, spot, spotCapacity, getSize);
            }

            void AddArrival(
                int spot,
                Vehicle vehicle,
                List<DemoOperation> target)
            {
                target.Add(new DemoOperation(
                    spot,
                    () => Park(vehicle, spot)));

                parkedVehicles.Add((spot, vehicle.Identifier));
            }

            void AddBatchSteps(
                IReadOnlyList<DemoOperation> operations,
                string label,
                int batchSize,
                int delayMs)
            {
                for (int i = 0; i < operations.Count; i += batchSize)
                {
                    DemoOperation[] batch = operations
                        .Skip(i)
                        .Take(batchSize)
                        .ToArray();

                    int startSpot = batch[0].Spot;
                    int lastSpot = batch[^1].Spot;

                    steps.Add(new DemoStep(
                        $"{label} | {batch.Length} vehicles, spots " +
                        $"{startSpot:D3}-{lastSpot:D3}",
                        lastSpot,
                        () =>
                        {
                            foreach (DemoOperation operation in batch)
                            {
                                operation.Execute();
                            }
                        },
                        delayMs));
                }
            }

            steps.Add(new DemoStep(
                "START | Empty 100-spot garage",
                1,
                () => { },
                650));

            // Initial demonstration: car, half-full MC spot and bicycles.
            var firstCar = new Car("DEMOCAR001", DateTime.UtcNow.AddMinutes(-65));
            var firstMc = new MC("DEMOMC002A", DateTime.UtcNow.AddMinutes(-45));
            var secondMc = new MC("DEMOMC002B", DateTime.UtcNow.AddMinutes(-30));
            var firstBicycle = new Bicycle("DEMOBK003A");

            steps.Add(new DemoStep(
                "CHECK-IN | Car DEMOCAR001 - spot 001 is full",
                1,
                () => Park(firstCar, 1),
                450));
            parkedVehicles.Add((1, firstCar.Identifier));

            steps.Add(new DemoStep(
                "CHECK-IN | MC DEMOMC002A - spot 002 is half full",
                2,
                () => Park(firstMc, 2),
                700));
            parkedVehicles.Add((2, firstMc.Identifier));

            steps.Add(new DemoStep(
                "CHECK-IN | Bicycle DEMOBK003A - spot 003 is partly occupied",
                3,
                () => Park(firstBicycle, 3),
                700));
            parkedVehicles.Add((3, firstBicycle.Identifier));

            steps.Add(new DemoStep(
                "CHECK-IN | Second MC shares spot 002 - now full",
                2,
                () => Park(secondMc, 2),
                550));
            parkedVehicles.Add((2, secondMc.Identifier));

            for (char letter = 'B'; letter <= 'D'; letter++)
            {
                var bicycle = new Bicycle($"DEMOBK003{letter}");
                parkedVehicles.Add((3, bicycle.Identifier));

                steps.Add(new DemoStep(
                    $"CHECK-IN | Bicycle {bicycle.Identifier} - spot 003",
                    3,
                    () => Park(bicycle, 3),
                    280));
            }

            // Bus reserves 4 spots, then moves with overlapping reservations.
            var bus = new Bus("DEMOBUS1", DateTime.UtcNow.AddMinutes(-95));
            steps.Add(new DemoStep(
                "BUS CHECK-IN | DEMOBUS1 occupies spots 004-007",
                4,
                () => garage.ParkBusAtSpot(
                    bus, 4, spotCapacity, getSize(bus), busLimit),
                950));

            steps.Add(new DemoStep(
                "BUS MOVE | DEMOBUS1 moves to spots 006-009",
                6,
                () => garage.MoveBus(
                    bus.Identifier, 6, spotCapacity, getSize(bus), busLimit),
                950));

            // Phase one: across the map, a balanced mix of full and orange spots.
            for (int spot = 4; spot <= DemoParkingSpots; spot++)
            {
                // Spots 006-009 are reserved for the bus throughout the fill.
                if (spot >= 6 && spot <= 9)
                {
                    continue;
                }

                switch (spot % 4)
                {
                    case 0:
                        AddArrival(
                            spot,
                            new Car($"DEMOCAR{spot:D3}"),
                            arrivals);
                        break;

                    case 1:
                        AddArrival(
                            spot,
                            new MC($"DEMOMC{spot:D3}A"),
                            arrivals);
                        AddArrival(
                            spot,
                            new MC($"DEMOMC{spot:D3}B"),
                            completions);
                        break;

                    case 2:
                        AddArrival(
                            spot,
                            new Bicycle($"DEMOBK{spot:D3}A"),
                            arrivals);

                        foreach (char letter in new[] { 'B', 'C', 'D' })
                        {
                            AddArrival(
                                spot,
                                new Bicycle($"DEMOBK{spot:D3}{letter}"),
                                completions);
                        }
                        break;

                    case 3:
                        AddArrival(
                            spot,
                            new MC($"DEMOMC{spot:D3}A"),
                            arrivals);
                        AddArrival(
                            spot,
                            new Bicycle($"DEMOBK{spot:D3}A"),
                            arrivals);
                        AddArrival(
                            spot,
                            new Bicycle($"DEMOBK{spot:D3}B"),
                            completions);
                        break;
                }
            }

            AddBatchSteps(
                arrivals,
                "CHECK-IN | Car, MC and Bicycle",
                batchSize: 8,
                delayMs: 150);

            // Cars, bikes and MCs are already mixed; demonstrate live departure
            // while new arrivals are still occurring later in the cycle.
            steps.Add(new DemoStep(
                "CHECKOUT | MC leaves spot 002 - half full again",
                2,
                () => garage.RemoveVehicle(firstMc.Identifier),
                650));

            steps.Add(new DemoStep(
                "CHECK-IN | MC returns to spot 002 - full again",
                2,
                () => Park(firstMc, 2),
                550));

            steps.Add(new DemoStep(
                "CHECKOUT | Bicycle leaves spot 003 - partly occupied",
                3,
                () => garage.RemoveVehicle(firstBicycle.Identifier),
                550));

            steps.Add(new DemoStep(
                "CHECK-IN | Bicycle returns to spot 003 - full again",
                3,
                () => Park(firstBicycle, 3),
                550));

            AddBatchSteps(
                completions,
                "CHECK-IN | Filling partial spots",
                batchSize: 8,
                delayMs: 150);

            steps.Add(new DemoStep(
                "GARAGE FULL | All 100 parking spots are occupied",
                100,
                () => { },
                1000));

            steps.Add(new DemoStep(
                "BUS CHECKOUT | Spots 006-009 released",
                6,
                () => garage.RemoveVehicle(bus.Identifier),
                900));

            // Genuine check-outs. Multiple vehicles per spot transition
            // through partially occupied (orange) before becoming green.
            var departures = parkedVehicles
                .OrderByDescending(entry => entry.Spot)
                .Select(entry => new DemoOperation(
                    entry.Spot,
                    () => garage.RemoveVehicle(entry.Identifier)))
                .ToList();

            AddBatchSteps(
                departures,
                "CHECK-OUT | Vehicles leaving",
                batchSize: 8,
                delayMs: 150);

            steps.Add(new DemoStep(
                "CYCLE COMPLETE | Garage empty - restarting",
                1,
                () => { },
                950));

            return steps;
        }
    }
}
