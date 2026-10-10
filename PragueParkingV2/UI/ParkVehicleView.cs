using PragueParkingV2.Data.Configuration;
using PragueParkingV2.Domain;
using Spectre.Console;

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
            AppConfiguration configuration)
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
                    vehicleType);

                if (parkingCompleted)
                {
                    return;
                }
            }
        }

        private static bool RegisterVehicle(
            ParkingGarage parkingGarage,
            AppConfiguration configuration,
            string vehicleType)
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

                    AppTheme.PrepareScreen();

                    MessageService.ShowSuccess(
                        $"Vehicle parked successfully!\n\n" +
                        $"Vehicle Type: {vehicleType}\n" +
                        $"Registration: {vehicle.Identifier}\n" +
                        $"Parking Spot: {assignedSpot.SpotNumber:D3}\n" +
                        $"Arrival Time: {localArrival:yyyy-MM-dd HH:mm}",
                        waitForKey: true);

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

            AnsiConsole.Write(panel);

            AnsiConsole.WriteLine();

            string identifier = AnsiConsole.Prompt(
                new TextPrompt<string>(
                    $"[{AppTheme.Gold}]Registration ID:[/] ")
                    .AllowEmpty());

            if (string.IsNullOrWhiteSpace(identifier))
            {
                return null;
            }

            return identifier.Trim();
        }
    }
}