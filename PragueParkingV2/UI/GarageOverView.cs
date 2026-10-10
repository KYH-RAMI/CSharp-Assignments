using PragueParkingV2.Data.Configuration;
using PragueParkingV2.Domain;
using Spectre.Console;
using Spectre.Console.Rendering;

namespace PragueParkingV2.UI
{
    internal static class GarageOverView
    {
        public static void DisplayGarageOverview(
            ParkingGarage parkingGarage,
            AppConfiguration configuration)
        {
            int currentPage = 1;
            const int spotsPerPage = 25;
            int selectedSpotIndex = 0;
            const int columnsPerRow = 5;
            int totalSpots = parkingGarage.ParkingSpots.Count;

            int totalPages = Math.Max(1, (int)Math.Ceiling(
                (double)totalSpots / spotsPerPage));

            while (true)
            {
                AppTheme.PrepareScreen();

                AnsiConsole.MarkupLine(
                    $"[bold {AppTheme.GoldLight}]GARAGE[/] " +
                    $"[bold {AppTheme.Gold}]OVER[/]" +
                    $"[bold {AppTheme.GoldDark}]VIEW[/]");

                AnsiConsole.WriteLine();

                var grid = new Grid();

                for (int column = 0; column < columnsPerRow; column++)
                {
                    grid.AddColumn(new GridColumn
                    {
                        Padding = new Padding(0, 0, 1, 0)
                    });
                }

                var pageSpots = parkingGarage.ParkingSpots
                    .OrderBy(spot => spot.SpotNumber)
                    .Skip((currentPage - 1) * spotsPerPage)
                    .Take(spotsPerPage)
                    .ToList();

                var selectedSpot = pageSpots[selectedSpotIndex];

                for (int row = 0; row < pageSpots.Count; row += columnsPerRow)
                {
                    IRenderable[] cells = new IRenderable[columnsPerRow];

                    for (int column = 0; column < columnsPerRow; column++)
                    {
                        int index = row + column;

                        if (index >= pageSpots.Count)
                        {
                            cells[column] = new Text("");
                            continue;
                        }

                        var spot = pageSpots[index];

                        bool isSelected = index == selectedSpotIndex;

                        string status;

                        if (spot.ParkedVehicles.Count == 0)
                        {
                            status = "[green]FREE[/]";
                        }
                        else
                        {
                            var firstVehicle = spot.ParkedVehicles[0];

                            var vehicleType = firstVehicle switch
                            {
                                Car => "Car",
                                MC => "MC",
                                _ => throw new NotSupportedException(
                                    $"Vehicle type '{firstVehicle.GetType().Name}' is not supported.")
                            };

                            var maxVehiclesPerSpot =
                                configuration.VehicleTypes![vehicleType].VehiclesPerSpot;

                            var color = spot.ParkedVehicles.Count < maxVehiclesPerSpot
                                ? "orange1"
                                : "red";

                            var occupancy = spot.ParkedVehicles.Count < maxVehiclesPerSpot
                                ? $"{vehicleType} {spot.ParkedVehicles.Count}/{maxVehiclesPerSpot}"
                                : "FULL";

                            status = $"[{color}]{occupancy}[/]";
                        }

                        var spotPanel = new Panel(
                            new Markup(
                                $"[bold]{spot.SpotNumber:D3}[/]\n{status}"))
                        {
                            Border = BoxBorder.Rounded,

                            BorderStyle = new Style(
                                foreground: isSelected ? Color.LightGoldenrod1 : Color.White),

                            Width = 12,

                            Padding = new Padding(1, 0, 1, 0)
                        };

                        cells[column] = spotPanel;
                    }

                    grid.AddRow(cells);
                }

                var detailLines = new List<string>
{
    $"[bold {AppTheme.Gold}]PARKING SPOT {selectedSpot.SpotNumber:D3}[/]",
    ""
};

                int vehicleCount = selectedSpot.ParkedVehicles.Count;

                if (vehicleCount == 0)
                {
                    detailLines.Add("Status: [green]FREE[/]");
                    detailLines.Add("Occupancy: 0");
                    detailLines.Add("");

                    detailLines.Add(
                        $"[bold {AppTheme.GoldLight}]AVAILABLE CAPACITY[/]");

                    detailLines.Add(
                        $"Car: {configuration.VehicleTypes!["Car"].VehiclesPerSpot}");

                    detailLines.Add(
                        $"MC:  {configuration.VehicleTypes!["MC"].VehiclesPerSpot}");
                }
                else
                {
                    var firstVehicle = selectedSpot.ParkedVehicles[0];

                    string vehicleType = firstVehicle switch
                    {
                        Car => "Car",
                        MC => "MC",
                        _ => throw new NotSupportedException(
                            $"Vehicle type '{firstVehicle.GetType().Name}' is not supported.")
                    };

                    int capacity =
                        configuration.VehicleTypes![vehicleType].VehiclesPerSpot;

                    bool isFull = vehicleCount >= capacity;

                    string status = isFull ? "FULL" : "PARTIALLY OCCUPIED";
                    string statusColor = isFull ? "red" : "orange1";

                    detailLines.Add($"Status: [{statusColor}]{status}[/]");
                    detailLines.Add($"Occupancy: {vehicleCount} / {capacity}");
                    detailLines.Add("");

                    var timeZone = TimeZoneInfo.FindSystemTimeZoneById(
                        configuration.System!.TimeZone!);

                    DateTime nowUtc = DateTime.UtcNow;

                    for (int i = 0; i < vehicleCount; i++)
                    {
                        var vehicle = selectedSpot.ParkedVehicles[i];

                        string type = vehicle switch
                        {
                            Car => "Car",
                            MC => "MC",
                            _ => throw new NotSupportedException(
                                $"Vehicle type '{vehicle.GetType().Name}' is not supported.")
                        };

                        DateTime localArrival = TimeZoneInfo.ConvertTimeFromUtc(
                            vehicle.ArrivalTime,
                            timeZone);

                        TimeSpan duration = nowUtc - vehicle.ArrivalTime;

                        detailLines.Add(
                            $"[bold {AppTheme.GoldLight}]VEHICLE {i + 1}[/]");

                        detailLines.Add($"Type: {type}");

                        detailLines.Add(
                            $"ID: {Markup.Escape(vehicle.Identifier)}");

                        detailLines.Add(
                            $"Arrival: {localArrival:yyyy-MM-dd HH:mm}");

                        detailLines.Add(
                            $"Duration: {FormatDuration(duration)}");

                        if (i < vehicleCount - 1)
                        {
                            detailLines.Add("");
                        }
                    }
                }

                var detailsPanel = new Panel(
                    new Markup(string.Join("\n", detailLines)))
                {
                    Header = new PanelHeader(
                        $"[bold {AppTheme.Gold}] SPOT DETAILS [/]"),

                    Border = BoxBorder.Rounded,

                    BorderStyle = new Style(
                        foreground: AppTheme.BorderColor),

                    Width = 40,

                    Padding = new Padding(1, 0, 1, 0)
                };

                var columns = new Columns(grid, detailsPanel)
                {
                    Expand = false,
                    Padding = new Padding(1, 0, 1, 0)
                };

                AnsiConsole.Write(columns);

                AnsiConsole.WriteLine();

                var footer = new Panel(
                    new Markup(
                        $"[bold {AppTheme.Gold}]PAGE {currentPage:00} / {totalPages:00}[/]" +
                        $"   [{AppTheme.Muted}]│[/]   " +
                        $"[{AppTheme.Text}]SPOTS {pageSpots[0].SpotNumber:D3}–{pageSpots[^1].SpotNumber:D3}[/]" +
                        $"   [{AppTheme.Muted}]│[/]   " +
                        $"[{AppTheme.Text}]TOTAL {totalSpots}[/]\n\n" +
                        $"[{AppTheme.GoldLight}]← ↑ ↓ →[/] " +
                        $"[{AppTheme.Muted}]Navigate[/]" +
                        $"   [{AppTheme.Muted}]│[/]   " +
                        $"[{AppTheme.GoldLight}]PgUp / PgDn[/] " +
                        $"[{AppTheme.Muted}]Pages[/]" +
                        $"   [{AppTheme.Muted}]│[/]   " +
                        $"[{AppTheme.GoldLight}]ESC[/] " +
                        $"[{AppTheme.Muted}]Back[/]"
                        ))
                {
                    Border = BoxBorder.Rounded,

                    BorderStyle = new Style(foreground: AppTheme.BorderColor),
                    Width = 80,
                    Padding = new Padding(2, 1, 2, 1)
                };

                AnsiConsole.Write(footer);

                var key = Console.ReadKey(intercept: true).Key;

                switch (key)
                {
                    case ConsoleKey.UpArrow
                    when selectedSpotIndex >= columnsPerRow:
                        selectedSpotIndex -= columnsPerRow;
                        break;

                    case ConsoleKey.DownArrow
                    when selectedSpotIndex + columnsPerRow < pageSpots.Count:
                        selectedSpotIndex += columnsPerRow;
                        break;

                    case ConsoleKey.LeftArrow
                    when selectedSpotIndex % columnsPerRow > 0:
                        selectedSpotIndex--;
                        break;

                    case ConsoleKey.RightArrow
                    when selectedSpotIndex % columnsPerRow < columnsPerRow - 1
                    && selectedSpotIndex + 1 < pageSpots.Count:
                        selectedSpotIndex++;
                        break;

                    case ConsoleKey.PageUp when currentPage > 1:
                        currentPage--;
                        selectedSpotIndex = 0;
                        break;

                    case ConsoleKey.PageDown when currentPage < totalPages:
                        currentPage++;
                        selectedSpotIndex = 0;
                        break;

                    case ConsoleKey.Escape:
                        return;
                }

            }

        }
        private static string FormatDuration(TimeSpan duration)
        {
            if (duration < TimeSpan.Zero)
            {
                return "Invalid time";
            }

            if (duration.Days > 0)
            {
                return $"{duration.Days}d {duration.Hours}h {duration.Minutes}m";
            }

            if (duration.Hours > 0)
            {
                return $"{duration.Hours}h {duration.Minutes}m";
            }

            return $"{duration.Minutes}m";
        }
    }
}
