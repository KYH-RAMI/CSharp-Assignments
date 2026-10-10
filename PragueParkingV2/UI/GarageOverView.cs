using PragueParkingV2.Data.Configuration;
using PragueParkingV2.Domain;
using Spectre.Console;
using Spectre.Console.Rendering;

namespace PragueParkingV2.UI
{
    internal static class GarageOverView
    {
        private const int SpotsPerPage = 25;
        private const int ColumnsPerRow = 5;

        public static void DisplayGarageOverview(
            ParkingGarage parkingGarage,
            AppConfiguration configuration)
        {
            int currentPage = 1;
            int selectedSpotIndex = 0;

            int totalSpots = parkingGarage.ParkingSpots.Count;

            int totalPages = Math.Max(
                1,
                (int)Math.Ceiling((double)totalSpots / SpotsPerPage));

            while (true)
            {
                AppTheme.PrepareScreen();

                AnsiConsole.MarkupLine(
                    AppTheme.GradientMarkup("GARAGE ", "OVER", "VIEW"));

                AnsiConsole.WriteLine();

                var pageSpots = parkingGarage.ParkingSpots
                    .OrderBy(spot => spot.SpotNumber)
                    .Skip((currentPage - 1) * SpotsPerPage)
                    .Take(SpotsPerPage)
                    .ToList();

                var selectedSpot = pageSpots[selectedSpotIndex];

                Grid garageGrid = BuildGarageGrid(
                    pageSpots,
                    selectedSpotIndex,
                    configuration);

                Panel detailsPanel = BuildDetailsPanel(
                    selectedSpot,
                    configuration);

                var columns = new Columns(garageGrid, detailsPanel)
                {
                    Expand = false,
                    Padding = new Padding(1, 0, 1, 0)
                };

                AnsiConsole.Write(columns);

                AnsiConsole.WriteLine();

                Panel footer = BuildFooter(
                    currentPage,
                    totalPages,
                    totalSpots,
                    pageSpots);

                AnsiConsole.Write(footer);

                ConsoleKey key =
                    Console.ReadKey(intercept: true).Key;

                switch (key)
                {
                    case ConsoleKey.UpArrow
                        when selectedSpotIndex >= ColumnsPerRow:

                        selectedSpotIndex -= ColumnsPerRow;
                        break;

                    case ConsoleKey.DownArrow
                        when selectedSpotIndex + ColumnsPerRow < pageSpots.Count:

                        selectedSpotIndex += ColumnsPerRow;
                        break;

                    case ConsoleKey.LeftArrow
                        when selectedSpotIndex % ColumnsPerRow > 0:

                        selectedSpotIndex--;
                        break;

                    case ConsoleKey.RightArrow
                        when selectedSpotIndex % ColumnsPerRow < ColumnsPerRow - 1
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

        private static Grid BuildGarageGrid(
            IReadOnlyList<ParkingSpot> pageSpots,
            int selectedSpotIndex,
            AppConfiguration configuration)
        {
            var grid = new Grid();

            for (int column = 0; column < ColumnsPerRow; column++)
            {
                grid.AddColumn(new GridColumn
                {
                    Padding = new Padding(0, 0, 1, 0)
                });
            }

            for (int row = 0; row < pageSpots.Count; row += ColumnsPerRow)
            {
                IRenderable[] cells = new IRenderable[ColumnsPerRow];

                for (int column = 0; column < ColumnsPerRow; column++)
                {
                    int index = row + column;

                    if (index >= pageSpots.Count)
                    {
                        cells[column] = new Text("");
                        continue;
                    }

                    ParkingSpot spot = pageSpots[index];

                    bool isSelected = index == selectedSpotIndex;

                    string status = GetSpotStatus(
                        spot,
                        configuration);

                    cells[column] = AppTheme.CreatePanel(
                        new Markup(
                            $"[bold]{spot.SpotNumber:D3}[/]\n{status}"),

                        width: AppTheme.GarageSpotWidth,

                        borderColor: isSelected
                            ? AppTheme.SelectedSpotBorder
                            : AppTheme.NormalSpotBorder);
                }

                grid.AddRow(cells);
            }

            return grid;
        }

        private static string GetSpotStatus(
            ParkingSpot spot,
            AppConfiguration configuration)
        {
            int vehicleCount = spot.ParkedVehicles.Count;

            if (vehicleCount == 0)
            {
                return $"[{AppTheme.StatusFree}]FREE[/]";
            }

            string vehicleType =
                GetVehicleType(spot.ParkedVehicles[0]);

            int capacity =
                configuration.VehicleTypes![vehicleType].VehiclesPerSpot;

            if (vehicleCount >= capacity)
            {
                return $"[{AppTheme.StatusFull}]FULL[/]";
            }

            return
                $"[{AppTheme.StatusPartial}]" +
                $"{vehicleType} {vehicleCount}/{capacity}[/]";
        }

        private static Panel BuildDetailsPanel(
            ParkingSpot selectedSpot,
            AppConfiguration configuration)
        {
            var detailLines = new List<string>
            {
                $"[bold {AppTheme.Gold}]" +
                $"PARKING SPOT {selectedSpot.SpotNumber:D3}[/]",
                ""
            };

            int vehicleCount = selectedSpot.ParkedVehicles.Count;

            if (vehicleCount == 0)
            {
                detailLines.Add(
                    $"Status: [{AppTheme.StatusFree}]FREE[/]");

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
                Vehicle firstVehicle = selectedSpot.ParkedVehicles[0];

                string vehicleType = GetVehicleType(firstVehicle);

                int capacity =
                    configuration.VehicleTypes![vehicleType].VehiclesPerSpot;

                bool isFull = vehicleCount >= capacity;

                string status = isFull
                    ? "FULL"
                    : "PARTIALLY OCCUPIED";

                string statusColor = isFull
                    ? AppTheme.StatusFull
                    : AppTheme.StatusPartial;

                detailLines.Add(
                    $"Status: [{statusColor}]{status}[/]");

                detailLines.Add(
                    $"Occupancy: {vehicleCount} / {capacity}");

                detailLines.Add("");

                TimeZoneInfo timeZone =
                    TimeZoneInfo.FindSystemTimeZoneById(
                        configuration.System!.TimeZone!);

                DateTime nowUtc = DateTime.UtcNow;

                for (int i = 0; i < vehicleCount; i++)
                {
                    Vehicle vehicle = selectedSpot.ParkedVehicles[i];

                    string type = GetVehicleType(vehicle);

                    DateTime localArrival =
                        TimeZoneInfo.ConvertTimeFromUtc(
                            vehicle.ArrivalTime,
                            timeZone);

                    TimeSpan duration =
                        nowUtc - vehicle.ArrivalTime;

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

            return AppTheme.CreatePanel(
                new Markup(string.Join("\n", detailLines)),
                title: "SPOT DETAILS",
                width: AppTheme.GarageDetailsWidth);
        }

        private static Panel BuildFooter(
            int currentPage,
            int totalPages,
            int totalSpots,
            IReadOnlyList<ParkingSpot> pageSpots)
        {
            string content =
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
                $"[{AppTheme.Muted}]Back[/]";

            return AppTheme.CreatePanel(
                new Markup(content),
                title: "GARAGE NAVIGATION",
                width: AppTheme.GarageFooterWidth,
                padding: new Padding(2, 1, 2, 1));
        }

        private static string GetVehicleType(Vehicle vehicle)
        {
            return vehicle switch
            {
                Car => "Car",
                MC => "MC",

                _ => throw new NotSupportedException(
                    $"Vehicle type '{vehicle.GetType().Name}' is not supported.")
            };
        }

        private static string FormatDuration(TimeSpan duration)
        {
            if (duration < TimeSpan.Zero)
            {
                return "Invalid time";
            }

            if (duration.Days > 0)
            {
                return
                    $"{duration.Days}d {duration.Hours}h {duration.Minutes}m";
            }

            if (duration.Hours > 0)
            {
                return $"{duration.Hours}h {duration.Minutes}m";
            }

            return $"{duration.Minutes}m";
        }
    }
}