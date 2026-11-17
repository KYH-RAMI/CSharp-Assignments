using PragueParkingV2.Data.Configuration;
using PragueParkingV2.Domain;
using Spectre.Console;

namespace PragueParkingV2.UI
{
    internal static class FindVehicleView
    {
        public static void Show(
            ParkingGarage parkingGarage,
            AppConfiguration configuration)
        {
            AppTheme.PrepareScreen();

            const int formWidth = 62;

            int leftMargin = Math.Max(0, (Console.WindowWidth - formWidth) / 2);

            Vehicle vehicle;

            while (true)
            {
                AppTheme.PrepareScreen();

                string instructionContent =
                    "Enter the vehicle registration ID to find its\n" +
                    "current location in the parking garage.\n\n" +
                    "Press Enter without an ID to return to the main menu.";

                var instructionPanel = AppTheme.CreatePanel(
                    new Markup(instructionContent),
                    title: "FIND VEHICLE",
                    width: formWidth);

                AppTheme.WriteCenteredPanel(instructionPanel);

                AnsiConsole.WriteLine();
                AnsiConsole.WriteLine();

                Console.SetCursorPosition(leftMargin, Console.CursorTop);

                string identifier = AnsiConsole.Prompt(
                    new TextPrompt<string>(
                        $"[{AppTheme.Gold}]Registration ID:[/] ")
                    .AllowEmpty()).Trim();

                if (string.IsNullOrWhiteSpace(identifier))
                {
                    return;
                }

                Vehicle? foundVehicle = parkingGarage.FindVehicle(identifier);

                if (foundVehicle is not null)
                {
                    vehicle = foundVehicle;
                    break;
                }

                AnsiConsole.WriteLine();

                MessageService.ShowError(
                    $"Vehicle '{Markup.Escape(identifier)}' was not found.");

                AnsiConsole.WriteLine();

                Console.SetCursorPosition(leftMargin, Console.CursorTop);

                bool tryAgain = AnsiConsole.Confirm(
                    $"[{AppTheme.Gold}]Try another registration ID?[/]",
                    defaultValue: true);

                if (!tryAgain)
                {
                    return;
                }
            }

            ParkingSpot? parkingSpot = parkingGarage.ParkingSpots
                .FirstOrDefault(spot => spot.ParkedVehicles.Contains(vehicle));
            
            if (parkingSpot is null)
            {
                MessageService.ShowError(
                    "The vehicle was found, but its parking spot could not be located.",
                    waitForKey: true);

                return;
            }

            string vehicleType = vehicle switch
            {
                Car => "Car",
                MC => "MC",
                _ => "Unknown"
            };
            
            TimeZoneInfo timeZone = TimeZoneInfo.FindSystemTimeZoneById(
                configuration.System!.TimeZone);

            DateTime localArrivalTime = TimeZoneInfo.ConvertTimeFromUtc(
                vehicle.ArrivalTime, timeZone);

            TimeSpan duration = DateTime.UtcNow - vehicle.ArrivalTime;

            string parkingDuration =
                $"{duration.Days}d {duration.Hours}h {duration.Minutes}m";

            string content =
                $"Vehicle Type: {vehicleType}\n" +
                $"Registration: {Markup.Escape(vehicle.Identifier)}\n" +
                $"Parking Spot: {parkingSpot.SpotNumber:D3}\n" +
                $"Arrival Time: {localArrivalTime:yyyy-MM-dd HH:mm}\n" +
                $"Duration: {parkingDuration}";

            AnsiConsole.WriteLine();

            var panel = AppTheme.CreatePanel(
                new Markup(content),
                title: "VEHICLE FOUND",
                width: 62,
                borderColor: AppTheme.SuccessBorderColor);

            AppTheme.WriteCenteredPanel(panel);

            AnsiConsole.WriteLine();

            Console.SetCursorPosition(leftMargin, Console.CursorTop);

            AnsiConsole.MarkupLine(
                $"[{AppTheme.Muted}]Press any key to continue...[/]");

            Console.ReadKey(intercept: true);
        }
    }
}