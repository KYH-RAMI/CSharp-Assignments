using PragueParkingV2_1.Data.Configuration;
using PragueParkingV2_1.Data.Persistence;
using PragueParkingV2_1.Domain;
using Spectre.Console;
using PragueParkingV2_1.Application;

namespace PragueParkingV2_1.UI
{
    internal static class MoveVehicleView
    {
        public static void Show(
            ParkingGarage parkingGarage,
            AppConfiguration configuration,
            ParkingDataService parkingDataService)
        {
            AppTheme.PrepareScreen();
            const int formWidth = 62;

            int leftMargin = Math.Max(
                0,
                (Console.WindowWidth - formWidth) / 2);

            string padding = new string(' ', leftMargin);

            string content =
                $"[{AppTheme.Muted}]" +
                $"Enter the vehicle registration number.\n" +
                $"Press ESC or leave empty and press Enter to go back." +
                $"[/]";

            var panel = AppTheme.CreatePanel(
                new Markup(content),
                title: "MOVE VEHICLE",
                width: 62);

            AppTheme.WriteCenteredPanel(panel);

            AnsiConsole.WriteLine();
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
                    $"Vehicle '{identifier}' was not found.");

                return;
            }
            var sourceSpot = parkingGarage.ParkingSpots.FirstOrDefault(
                spot => spot.ParkedVehicles.Contains(vehicle));
            if (sourceSpot is null)
            {
                MessageService.ShowError(
                    $"Parking spot for '{identifier}' was not found.");

                return;
            }
            string sourceSpotText = vehicle is Bus
                ? $"{sourceSpot.SpotNumber:D3} - {sourceSpot.SpotNumber + 3:D3}"
                : $"{sourceSpot.SpotNumber:D3}";
            AnsiConsole.WriteLine();
            string currentspot =
                $"\n[bold {AppTheme.GoldLight}]VEHICLE FOUND[/]\n\n" +
                $"[{AppTheme.Muted}]" +
                $"Registration: {Markup.Escape(identifier)}\n" +
                $"Current Spot(s): {sourceSpotText}\n" +
                $"[/]";

            var spotpanel = AppTheme.CreatePanel(
               new Markup(currentspot),
               title: "STATUS OF VEHICLE",
               width: 62);

            AppTheme.WriteCenteredPanel(spotpanel);

            AnsiConsole.WriteLine();
            AnsiConsole.WriteLine();

            string newspot =
                $"[{AppTheme.Muted}]" +
                $"Enter the new SPOT number to move vehicle to.\n" +
                $"Press ESC or leave empty and press Enter to go back." +
                $"[/]";

            var newspotpanel = AppTheme.CreatePanel(
                new Markup(newspot),
                title: "MOVE VEHICLE",
                width: 62);
            AnsiConsole.WriteLine();
            AppTheme.WriteCenteredPanel(newspotpanel);
            AnsiConsole.WriteLine();
            AnsiConsole.WriteLine();

            int spotCapacity = configuration.Garage!.SpotCapacity;

            Func<IVehicle, int> getSize = v =>
            configuration.VehicleTypes![v.GetType().Name].Size;

            int targetSpotNumber;

            // Sparar positionen där Spot-inmatningen börjar.
            int spotAreaTop = Console.CursorTop;

            // Innehåller det senaste felmeddelandet.
            string? errorMessage = null;

            while (true)
            {
                // Gå tillbaka till början av inmatningsområdet.
                Console.SetCursorPosition(0, spotAreaTop);

                // Rensa endast området nedanför markören.
                Console.Write("\x1b[0J");

                // Visa det senaste felet, om något finns.
                if (errorMessage is not null)
                {
                    MessageService.ShowError(errorMessage);
                    AnsiConsole.WriteLine();
                }
                Console.SetCursorPosition(
                    leftMargin,
                    Console.CursorTop);

                string? spot = MenuInput.ReadText(
                    "Spot",
                    leftMargin,
                    maxLength: 10);

                // Avbryt flytten.
                if (string.IsNullOrWhiteSpace(spot))
                {
                    return;
                }

                // Kontrollera att användaren angav ett heltal.
                if (!int.TryParse(spot, out targetSpotNumber))
                {
                     errorMessage =
                        "Please enter a valid parking spot number.";

                    continue;
                }

                // Kontrollera att platsen finns.
                if (targetSpotNumber < 1 ||
                    targetSpotNumber > parkingGarage.ParkingSpots.Count)
                {
                    errorMessage =
                        $"Parking spot must be between 1 and {parkingGarage.ParkingSpots.Count}.";
                  
                    continue;
                }

                // Kontrollera att fordonet inte redan står där.
                if (targetSpotNumber == sourceSpot.SpotNumber)
                {
                    errorMessage =
                        $"Vehicle '{identifier}' is already parked at spot {targetSpotNumber:D3}.";
                 
                    continue;
                }
                // Kontrollera att destinationsplatsen kan ta emot fordonet.
                bool canMoveToTarget;

                if (vehicle is Bus bus)
                {
                    canMoveToTarget = parkingGarage.CanMoveBusToSpot(
                        bus,
                        targetSpotNumber,
                        configuration.Garage!.BusMaxParkingSpot);
                }
                else
                {
                    var targetSpot = parkingGarage.ParkingSpots.FirstOrDefault(
                        spot => spot.SpotNumber == targetSpotNumber);

                    canMoveToTarget =
                        targetSpot is not null &&
                        targetSpot.CanPark(vehicle, spotCapacity, getSize);
                }

                if (!canMoveToTarget)
                {
                    errorMessage =
                        $"Parking spot {targetSpotNumber:D3} is not available for this vehicle.";

                    continue;
                }

                break;


            }
            Console.SetCursorPosition(0, spotAreaTop);
            Console.Write("\x1b[0J");
            
            string targetSpotText = vehicle is Bus
                ? $"{targetSpotNumber:D3} - {targetSpotNumber + 3:D3}"
                : $"{targetSpotNumber:D3}";

            string summary =
                $"[{AppTheme.Muted}]" +
                $"Registration: {Markup.Escape(identifier)}\n" +
                $"Current Spot(s): {sourceSpotText}\n" +
                $"Target Spot(s): {targetSpotText}" +
                $"[/]";
            
            var confirmationPanel = AppTheme.CreatePanel(
                new Markup(summary),
                title: "MOVE CONFIRMATION",
                width: 62);
            
            AppTheme.WriteCenteredPanel(confirmationPanel);
            AnsiConsole.WriteLine();

            Console.SetCursorPosition(
                leftMargin,
                Console.CursorTop);

            bool confirmed = MenuInput.Confirm(
                "Confirm move?",
                leftMargin);

            if (!confirmed)
            {
                return;
            }


            try
            {
                if (vehicle is Bus bus)
                {
                    parkingGarage.MoveBus(
                        identifier,
                        targetSpotNumber,
                        spotCapacity,
                        getSize(bus),
                        configuration.Garage!.BusMaxParkingSpot);
                }
                else
                {
                    parkingGarage.MoveVehicle(
                        identifier,
                        targetSpotNumber,
                        spotCapacity,
                        getSize);
                }
            }
            catch (InvalidOperationException ex)
            {
                MessageService.ShowError(
                    ex.Message,
                    waitForKey: true);

                return;
            }

            try
            {
                var document =
                    ParkingDataMapper.ToDocument(parkingGarage);

                parkingDataService.Save(document);
            }
            catch (Exception ex) when (
                ex is IOException or UnauthorizedAccessException)
            {
                // Återställ fordonets ursprungliga placering.
                try
                {
                    if (vehicle is Bus bus)
                    {
                        parkingGarage.MoveBus(
                            identifier,
                            sourceSpot.SpotNumber,
                            spotCapacity,
                            getSize(bus),
                            configuration.Garage!.BusMaxParkingSpot);
                    }
                    else
                    {
                        parkingGarage.MoveVehicle(
                            identifier,
                            sourceSpot.SpotNumber,
                            spotCapacity,
                            getSize);
                    }
                }
                catch (Exception rollbackEx)
                {
                    MessageService.ShowError(
                        $"Failed to save parking data: {ex.Message}\n" +
                        $"Could not restore the original parking spot: {rollbackEx.Message}",
                        waitForKey: true);

                    return;
                }

                MessageService.ShowError(
                    $"Failed to save parking data: {ex.Message}\n" +
                    "The vehicle has been returned to its original spot.",
                    waitForKey: true);

                return;
            }

            AppTheme.PrepareScreen();

            string successContent =
                $"Vehicle moved successfully!\n\n" +
                $"Vehicle Type: {vehicle.GetType().Name}\n" +
                $"Registration: {Markup.Escape(identifier)}\n" +
                $"Previous Spot(s): {sourceSpotText}\n" +
                $"New Spot(s): {targetSpotText}";

            var successPanel = AppTheme.CreatePanel(
                new Markup(successContent),
                title: "MOVE COMPLETED",
                width: 62,
                borderColor: AppTheme.SuccessBorderColor);

            AppTheme.WriteCenteredPanel(successPanel);

            AnsiConsole.WriteLine();

            Console.SetCursorPosition(
                leftMargin,
                Console.CursorTop);

            AnsiConsole.MarkupLine(
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
