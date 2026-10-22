using PragueParkingV2_1.Application;
using PragueParkingV2_1.Data.Configuration;
using PragueParkingV2_1.Data.Persistence;
using PragueParkingV2_1.Domain;
using Spectre.Console;

namespace PragueParkingV2_1.UI
{
    internal static class CheckOutVehicleView
    {
        public static void Show(
            ParkingGarage parkingGarage,
            AppConfiguration configuration,
            ParkingDataService parkingDataService,
            ParkingFeeCalculator feeCalculator)
        {
            AppTheme.PrepareScreen();
            const int formWidth = 62;

            int leftMargin = Math.Max(
                0,
                (Console.WindowWidth - formWidth) / 2);

            string padding = new string(' ', leftMargin);

            string content =
                $"[bold {AppTheme.GoldLight}]CHECK OUT VEHICLE[/]\n" +
                $"[{AppTheme.Muted}]" +
                $"Enter the vehicle registration number.\n" +
                $"Press ESC or leave empty and press Enter to go back." +
                $"[/]";

            var panel = AppTheme.CreatePanel(
                new Markup(content),
                title: "VEHICLE CHECKOUT",
                width: 62);

            AppTheme.WriteCenteredPanel(panel);

            AnsiConsole.WriteLine();
            Console.SetCursorPosition(
                leftMargin,
                Console.CursorTop);

            string? identifier = MenuInput.ReadText(
                "Registration ID",
                leftMargin);

            if (string.IsNullOrWhiteSpace(identifier))
            {
                return;
            }

            identifier = identifier.Trim();


            var vehicle = parkingGarage.FindVehicle(identifier);
            if (vehicle is null)
            {
                MessageService.ShowError(
                    $"Vehicle '{identifier}' was not found.",
                    waitForKey: true);

                return;
            }


            DateTime departureTime = DateTime.UtcNow;

            var fee = feeCalculator.CalculateParkingFee(
                vehicle,
                departureTime);
            var roundedMinutes = Math.Ceiling(
                fee.ParkingDuration.TotalMinutes);
            var duration = TimeSpan.FromMinutes(roundedMinutes);
            
            string formattedDuration = duration.Days > 0
                ? $"{duration.Days}d {duration.Hours}h {duration.Minutes}m"
                : duration.Hours > 0
                ? $"{duration.Hours}h {duration.Minutes}m"
                : $"{duration.Minutes}m";

            string summary =
                $"Vehicle: {Markup.Escape(vehicle.Identifier)}\n" +
                $"Duration: {formattedDuration}\n" +
                $"Hourly price: {fee.HourlyPrice} CZK\n" +
                $"Free minutes: {fee.FreeMinutes}\n\n" +
                $"[bold {AppTheme.Gold}]TOTAL FEE: {fee.TotalFee} CZK[/]";

            var summaryPanel = AppTheme.CreatePanel(
                new Markup(summary),
                title: "CHECKOUT SUMMARY",
                width: 62);

            AnsiConsole.WriteLine();

            AppTheme.WriteCenteredPanel(summaryPanel);

            AnsiConsole.WriteLine();
            Console.SetCursorPosition(
                leftMargin,
                Console.CursorTop);

            bool confirmed = MenuInput.Confirm(
                "Confirm checkout?",
                leftMargin);

            if (!confirmed)
            {
                MessageService.ShowInfo(
                    "Checkout cancelled.");

                return;
            }
            var originalSpot = parkingGarage.ParkingSpots
                .FirstOrDefault(spot => spot.ParkedVehicles.Contains(vehicle));

            if (originalSpot is null)
            {
                MessageService.ShowError(
                    "The vehicle's parking spot could not be found.",
                    waitForKey: true);

                return;
            }

            int spotCapacity = configuration.Garage!.SpotCapacity;

            Func<IVehicle, int> getSize = vehicle =>
                configuration.VehicleTypes![vehicle.GetType().Name].Size;

            var checkoutService = new CheckoutService(
                parkingGarage,
                feeCalculator);
            var checkoutResult = checkoutService.CheckOut(
                identifier,
                departureTime);

            try
            {
                var document =
                    ParkingDataMapper.ToDocument(parkingGarage);

                parkingDataService.Save(document);
            }
            catch (Exception ex) when (
                ex is IOException or UnauthorizedAccessException)
            {
                if (vehicle is Bus bus)
                {
                    parkingGarage.ParkBusAtSpot(
                        bus,
                        originalSpot.SpotNumber,
                        spotCapacity,
                        getSize(bus),
                        configuration.Garage!.BusMaxParkingSpot);
                }
                else
                {
                    parkingGarage.ParkVehicleAtSpot(
                        vehicle,
                        originalSpot.SpotNumber,
                        spotCapacity,
                        getSize);
                }

                AppTheme.PrepareScreen();

                MessageService.ShowError(
                    $"Failed to update parking data: {ex.Message}\n" +
                    "The parking checkout has been cancelled.",
                    waitForKey: true);

                return;
            }

            AppTheme.PrepareScreen();
            string checkoutSummary =
                $"Vehicle checked out successfully!\n\n" +
                $"Registration: {checkoutResult.Identifier}\n" +
                $"Duration: {formattedDuration}\n" +
                $"Hourly price: {checkoutResult.HourlyPrice} CZK\n" +
                $"Free minutes: {checkoutResult.FreeMinutes}\n" +
                $"TOTAL FEE: {checkoutResult.TotalFee} CZK";

            var checkoutSummaryPanel = AppTheme.CreatePanel(
               new Markup(checkoutSummary),
               title: "CHECKOUT COMPLETED",
               width: 62,
               borderColor: AppTheme.SuccessBorderColor);

            AnsiConsole.WriteLine();
            AppTheme.WriteCenteredPanel(checkoutSummaryPanel);
            AnsiConsole.WriteLine();

            AnsiConsole.WriteLine();

             leftMargin = Math.Max(
                0,
                (Console.WindowWidth - AppTheme.FormWidth) / 2);

            AnsiConsole.MarkupLine(
                $"{new string(' ', leftMargin)}" +
                $"[{AppTheme.Muted}]Press any key to continue...[/]");
            Thread.Sleep(3000);
            while (Console.KeyAvailable)
            {
                Console.ReadKey(intercept: true);
            }
            Console.ReadKey(intercept: true);

        }
    }
}
