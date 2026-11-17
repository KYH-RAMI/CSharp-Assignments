using PragueParkingV2.Data.Configuration;
using PragueParkingV2.Domain;
using Spectre.Console;
using Spectre.Console.Rendering;

namespace PragueParkingV2.UI
{
    internal static class GarageOverView
    {
        // The map is measured from the terminal, not from a fixed garage capacity.
        private static (int MapWidth, int DetailsWidth, int ColumnsPerRow, int VisibleRows)
            GetGarageLayout(int numberOfSpots)
        {
            int terminalWidth = AnsiConsole.Console.Profile.Width;
            int terminalHeight = AnsiConsole.Console.Profile.Height;

            // Leave the last terminal column unused to avoid automatic line wrapping.
            int availableWidth = Math.Max(1, terminalWidth - 1);

            // Details uses about 20 percent, but needs a readable minimum width.
            int detailsWidth = Math.Max(23, availableWidth / 5);
            int mapWidth = availableWidth - detailsWidth - 1;

            // The panel has a border (2) and horizontal padding (2).
            int mapContentWidth = Math.Max(1, mapWidth - 4);

            // Each number uses two colored spaces and one separating space.
            int numberWidth = Math.Max(3, numberOfSpots.ToString().Length);
            int cellWidth = numberWidth + 3;

            int columnsPerRow = Math.Max(
                1,
                Math.Min(numberOfSpots, mapContentWidth / cellWidth));

            // Footer, panel borders, and a safety margin are reserved below the map.
            int visibleRows = Math.Max(1, terminalHeight - 12);

            return (mapWidth, detailsWidth, columnsPerRow, visibleRows);
        }

        public static void DisplayGarageOverview(
            ParkingGarage parkingGarage,
            AppConfiguration configuration)
        {
            var garageSpots = parkingGarage.ParkingSpots
                .OrderBy(spot => spot.SpotNumber)
                .ToList();

            if (garageSpots.Count == 0)
            {
                AnsiConsole.MarkupLine("[yellow]No parking spots are configured.[/]");
                return;
            }

            // A smaller terminal cannot show both panels and the footer readably.
            if (AnsiConsole.Console.Profile.Width < 80 ||
                AnsiConsole.Console.Profile.Height < 18)
            {
                AnsiConsole.MarkupLine(
                    "[yellow]Enlarge the terminal to at least 80 columns and 18 rows.[/]");
                return;
            }

            int selectedSpotIndex = 0;
            var layout = GetGarageLayout(garageSpots.Count);

            AppTheme.PrepareScreen();

            AnsiConsole.Live(
                BuildOverviewLayout(
                    garageSpots,
                    selectedSpotIndex,
                    parkingGarage,
                    configuration,
                    layout))
                .Start(ctx =>
                {
                    ctx.Refresh();

                    while (true)
                    {
                        ConsoleKey key = Console.ReadKey(intercept: true).Key;

                        if (key == ConsoleKey.Escape)
                        {
                            return;
                        }

                        // Recalculate on each keypress, including after a terminal resize.
                        layout = GetGarageLayout(garageSpots.Count);
                        int columnsPerRow = layout.ColumnsPerRow;

                        switch (key)
                        {
                            case ConsoleKey.UpArrow
                                when selectedSpotIndex >= columnsPerRow:
                                selectedSpotIndex -= columnsPerRow;
                                break;

                            case ConsoleKey.DownArrow
                                when selectedSpotIndex + columnsPerRow < garageSpots.Count:
                                selectedSpotIndex += columnsPerRow;
                                break;

                            case ConsoleKey.LeftArrow
                                when selectedSpotIndex % columnsPerRow > 0:
                                selectedSpotIndex--;
                                break;

                            case ConsoleKey.RightArrow
                                when selectedSpotIndex % columnsPerRow < columnsPerRow - 1 &&
                                     selectedSpotIndex + 1 < garageSpots.Count:
                                selectedSpotIndex++;
                                break;
                        }

                        ctx.UpdateTarget(
                            BuildOverviewLayout(
                                garageSpots,
                                selectedSpotIndex,
                                parkingGarage,
                                configuration,
                                layout));
                    }
                });
        }

        private static IRenderable BuildOverviewLayout(
            IReadOnlyList<ParkingSpot> garageSpots,
            int selectedSpotIndex,
            ParkingGarage parkingGarage,
            AppConfiguration configuration,
            (int MapWidth, int DetailsWidth, int ColumnsPerRow, int VisibleRows) layout)
        {
            // A viewport is used only if the complete map exceeds the terminal height.
            int totalRows = (garageSpots.Count + layout.ColumnsPerRow - 1) /
                            layout.ColumnsPerRow;
            int visibleRows = Math.Min(totalRows, layout.VisibleRows);
            int selectedRow = selectedSpotIndex / layout.ColumnsPerRow;
            int firstVisibleRow = Math.Clamp(
                selectedRow - visibleRows / 2,
                0,
                Math.Max(0, totalRows - visibleRows));

            int firstVisibleIndex = firstVisibleRow * layout.ColumnsPerRow;
            int endVisibleIndex = Math.Min(
                garageSpots.Count,
                (firstVisibleRow + visibleRows) * layout.ColumnsPerRow);

            Panel garagePanel = BuildGaragePanel(
                garageSpots,
                selectedSpotIndex,
                configuration,
                layout.MapWidth,
                layout.ColumnsPerRow,
                firstVisibleIndex,
                endVisibleIndex);

            Panel detailsPanel = BuildDetailsPanel(
                garageSpots[selectedSpotIndex],
                configuration,
                layout.DetailsWidth);

            int totalWidth = layout.MapWidth + 1 + layout.DetailsWidth;
            Panel footer = BuildFooter(
                parkingGarage,
                totalWidth,
                garageSpots[firstVisibleIndex].SpotNumber,
                garageSpots[endVisibleIndex - 1].SpotNumber);

            var top = new Grid();
            top.AddColumn(new GridColumn
            {
                Width = layout.MapWidth,
                Padding = new Padding(0, 0, 0, 0)
            });
            top.AddColumn(new GridColumn
            {
                Width = 1,
                Padding = new Padding(0, 0, 0, 0)
            });
            top.AddColumn(new GridColumn
            {
                Width = layout.DetailsWidth,
                Padding = new Padding(0, 0, 0, 0)
            });
            top.AddRow(garagePanel, new Text(""), detailsPanel);

            // Footer spans both panels rather than occupying only the first column.
            return new Rows(top, footer);
        }

        private static Grid BuildGarageGrid(
            IReadOnlyList<ParkingSpot> garageSpots,
            int selectedSpotIndex,
            AppConfiguration configuration,
            int columnsPerRow,
            int firstVisibleIndex,
            int endVisibleIndex)
        {
            int numberWidth = Math.Max(3, garageSpots[^1].SpotNumber.ToString().Length);
            int cellWidth = numberWidth + 3;

            var grid = new Grid();

            for (int column = 0; column < columnsPerRow; column++)
            {
                grid.AddColumn(new GridColumn
                {
                    Width = cellWidth,
                    NoWrap = true,
                    Padding = new Padding(0, 0, 0, 0)
                });
            }

            for (int row = firstVisibleIndex; row < endVisibleIndex;
                 row += columnsPerRow)
            {
                var cells = new IRenderable[columnsPerRow];

                for (int column = 0; column < columnsPerRow; column++)
                {
                    int index = row + column;

                    cells[column] = index < endVisibleIndex
                        ? CreateParkingSpotCell(
                            garageSpots[index],
                            index == selectedSpotIndex,
                            configuration)
                        : new Text("");
                }

                grid.AddRow(cells);
            }

            return grid;
        }

        private static Panel BuildGaragePanel(
            IReadOnlyList<ParkingSpot> garageSpots,
            int selectedSpotIndex,
            AppConfiguration configuration,
            int mapWidth,
            int columnsPerRow,
            int firstVisibleIndex,
            int endVisibleIndex)
        {
            Grid map = BuildGarageGrid(
                garageSpots,
                selectedSpotIndex,
                configuration,
                columnsPerRow,
                firstVisibleIndex,
                endVisibleIndex);

            return AppTheme.CreatePanel(
                map,
                title: "PARKING MAP",
                width: mapWidth,
                padding: new Padding(1, 0, 1, 0));
        }

        private static Markup CreateParkingSpotCell(
            ParkingSpot spot,
            bool isSelected,
            AppConfiguration configuration)
        {
            string backgroundColor = GetSpotColor(spot, configuration);
            string foregroundColor = isSelected
                ? AppTheme.Gold
                : AppTheme.StatusText;

            // The final space is deliberately outside the colored area.
            return new Markup(
                $"[bold {foregroundColor} on {backgroundColor}] " +
                $"{spot.SpotNumber:D3} [/] ");
        }

        private static string GetSpotColor(
            ParkingSpot parkingSpot,
            AppConfiguration configuration)
        {
            int vehicleCount =
                parkingSpot.ParkedVehicles.Count;


            if (vehicleCount == 0)
            {
                return AppTheme.StatusFree;
            }


            string vehicleType =
                GetVehicleType(
                    parkingSpot.ParkedVehicles[0]);


            int capacity =
                configuration.VehicleTypes![vehicleType]
                    .VehiclesPerSpot;


            if (vehicleCount >= capacity)
            {
                return AppTheme.StatusFull;
            }


            return AppTheme.StatusPartial;
        }
        private static Panel BuildDetailsPanel(
            ParkingSpot selectedSpot,
            AppConfiguration configuration,
            int detailsWidth)
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
                width: detailsWidth,
                padding: new Padding(1, 0, 1, 0));
        }

        private static Panel BuildFooter(
            ParkingGarage parkingGarage,
            int width,
            int firstVisibleSpot,
            int lastVisibleSpot)
        {
            int totalSpots =
                parkingGarage.ParkingSpots.Count;

            int occupiedSpots =
                parkingGarage.ParkingSpots.Count(
                    spot => spot.ParkedVehicles.Count > 0);

            int freeSpots =
                totalSpots - occupiedSpots;

            int totalVehicles =
                parkingGarage.ParkingSpots.Sum(
                    spot => spot.ParkedVehicles.Count);

            string content =
                $"[bold {AppTheme.Gold}]FREE {freeSpots}[/]" +
                $"   [{AppTheme.Muted}]│[/]   " +
                $"[{AppTheme.Text}]OCCUPIED {occupiedSpots}[/]" +
                $"   [{AppTheme.Muted}]│[/]   " +
                $"[{AppTheme.Text}]VEHICLES {totalVehicles}[/]\n\n" +

                $"[{AppTheme.GoldLight}]← ↑ ↓ →[/] " +
                $"[{AppTheme.Muted}]Navigate[/]" +
                $"   [{AppTheme.Muted}]│[/]   " +
                $"[{AppTheme.GoldLight}]ESC[/] " +
                $"[{AppTheme.Muted}]Back[/]";

            if (firstVisibleSpot > 1 || lastVisibleSpot < totalSpots)
            {
                content +=
                    $"\n[{AppTheme.Muted}]Showing spots " +
                    $"{firstVisibleSpot:D3}–{lastVisibleSpot:D3} of {totalSpots}[/]";
            }

            return AppTheme.CreatePanel(
                new Markup(content),
                title: "GARAGE STATUS",
                width: width,
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