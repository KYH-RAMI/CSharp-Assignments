using PragueParkingV2.Data.Configuration;
using PragueParkingV2.Data.Persistence;
using PragueParkingV2.Domain;
using Spectre.Console;
using PragueParkingV2.Application;

namespace PragueParkingV2.UI
{
    internal static class ParkVehicleView
    {
        private static readonly string[] VehicleOptions =
        {
            "Car",
            "MC",
            "Back"
        };

        public static void Show(
            ParkingGarage parkingGarage,
            AppConfiguration configuration,
            ParkingDataService parkingDataService)
        {
            int selectedIndex = 0;

            while (true)
            {
                string vehicleType = AppTheme.ShowMenu(
                    title: "PARK A VEHICLE",
                    subtitle: "SELECT VEHICLE TYPE",
                    options: VehicleOptions,
                    selectedIndex: ref selectedIndex,
                    escapeOption: "Back",
                    description: "NEW PARKING REGISTRATION",
                    hint: "Choose a vehicle type to continue");

                if (vehicleType == "Back")
                {
                    return;
                }

                bool parkingCompleted = RegisterVehicle(
                    parkingGarage,
                    configuration,
                    vehicleType,
                    parkingDataService);

                if (parkingCompleted)
                {
                    return;
                }
            }
        }

        private static bool RegisterVehicle(
            ParkingGarage parkingGarage,
            AppConfiguration configuration,
            string vehicleType,
            ParkingDataService parkingDataService)
        {
            while (true)
            {
                string? identifier = ReadRegistrationId(vehicleType);

                if (identifier is null)
                {
                    return false;
                }

                try
                {
                    Vehicle vehicle = vehicleType switch
                    {
                        "Car" => new Car(identifier),
                        "MC" => new MC(identifier),

                        _ => throw new ArgumentException(
                            "Unsupported vehicle type.")
                    };

                    int capacity =
                        configuration.VehicleTypes![vehicleType]
                            .VehiclesPerSpot;

                    TimeZoneInfo timeZone =
                        TimeZoneInfo.FindSystemTimeZoneById(
                            configuration.System!.TimeZone!);

                    DateTime localArrival =
                        TimeZoneInfo.ConvertTimeFromUtc(
                            vehicle.ArrivalTime,
                            timeZone);

                    ParkingSpot assignedSpot =
                        parkingGarage.ParkVehicle(
                            vehicle,
                            capacity);
                    try
                    {
                        var document = ParkingDataMapper.ToDocument(parkingGarage);

                        parkingDataService.Save(document);
                    }
                    catch (Exception ex) when (
                        ex is IOException or UnauthorizedAccessException)
                    {
                        parkingGarage.RemoveVehicle(vehicle.Identifier);

                        AppTheme.PrepareScreen();

                        MessageService.ShowError(
                            $"Failed to save parking data: {ex.Message}\n" +
                            "The parking registration has been cancelled.",
                            waitForKey: true);

                        return false;
                    }

                    AppTheme.PrepareScreen();

                    string successContent =
                        $"Vehicle parked successfully!\n\n" +
                        $"Vehicle Type: {vehicleType}\n" +
                        $"Registration: {vehicle.Identifier}\n" +
                        $"Parking Spot: {assignedSpot.SpotNumber:D3}\n" +
                        $"Arrival Time: {localArrival:yyyy-MM-dd HH:mm}";

                    var successPanel = AppTheme.CreatePanel(
                        new Markup(successContent),
                        title: "PARKING COMPLETED",
                        width: 62,
                        borderColor: AppTheme.SuccessBorderColor);

                    AppTheme.WriteCenteredPanel(successPanel);
                    AnsiConsole.WriteLine();

                    int leftMargin = Math.Max(
                        0,
                        (Console.WindowWidth - 62) / 2);

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

                    return true;
                }
                catch (ArgumentException ex)
                {
                    AppTheme.PrepareScreen();

                    MessageService.ShowError(
                        ex.Message,
                        waitForKey: true);
                }
                catch (InvalidOperationException ex)
                {
                    AppTheme.PrepareScreen();

                    MessageService.ShowError(
                        ex.Message,
                        waitForKey: true);
                }
            }
        }

        private static string? ReadRegistrationId(string vehicleType)
        {
            AppTheme.PrepareScreen();
            const int formWidth = 62;
            int leftMargin = Math.Max(
                0,
                (Console.WindowWidth - formWidth) / 2);

            string padding = new string(' ', leftMargin);
            string content =
                $"[bold {AppTheme.GoldLight}]VEHICLE TYPE[/]\n" +
                $"{Markup.Escape(vehicleType)}\n\n" +
                $"[{AppTheme.Muted}]" +
                $"Enter the vehicle registration number.\n" +
                $"Leave empty and press Enter to go back." +
                $"[/]";

            var panel = AppTheme.CreatePanel(
                new Markup(content),
                title: "VEHICLE REGISTRATION",
                width: 62);

            AppTheme.WriteCenteredPanel(panel);

            AnsiConsole.WriteLine();

            Console.SetCursorPosition(
                leftMargin,
                Console.CursorTop);

            string identifier = AnsiConsole.Prompt(
                new TextPrompt<string>(
                    $"{padding}[{AppTheme.Gold}]Registration ID:[/] ")
                    .AllowEmpty());

            if (string.IsNullOrWhiteSpace(identifier))
            {
                return null;
            }

            return identifier.Trim();
        }
    }
}