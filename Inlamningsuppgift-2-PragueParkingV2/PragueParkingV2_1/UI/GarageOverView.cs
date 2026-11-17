using PragueParkingV2_1.Data.Configuration;
using PragueParkingV2_1.Domain;
using Spectre.Console;
using Spectre.Console.Rendering;
using PragueParkingV2_1.Application;
namespace PragueParkingV2_1.UI
{
    internal static class GarageOverView
    {
        // Layout dimensions are based on terminal size, not garage capacity.
        private static (int MapWidth, int DetailsWidth, int ColumnsPerRow, int VisibleRows)
            GetGarageLayout(int numberOfSpots, bool demoMode = false)
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

            // Reserve room for details, the footer and the demo event panel.
            int visibleRows = Math.Max(1, terminalHeight - (demoMode ? 15 : 10));

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
                AnsiConsole.Console.Profile.Height < 24)
            {
                AnsiConsole.MarkupLine(
                    "[yellow]Enlarge the terminal to at least 80 columns and 24 rows.[/]");
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
        // Runs a separate, in-memory garage until ESC is pressed.
        public static void DisplayLiveDemonstration(
            AppConfiguration configuration)
        {
            ArgumentNullException.ThrowIfNull(configuration);

            if (AnsiConsole.Console.Profile.Width < 80 ||
                AnsiConsole.Console.Profile.Height < 28)
            {
                AnsiConsole.MarkupLine(
                    "[yellow]Enlarge the terminal to at least 80 columns and 28 rows.[/]");
                return;
            }

            ParkingGarage demoGarage = new(
                DemoScenario.DemoParkingSpots);

            List<ParkingSpot> garageSpots = demoGarage.ParkingSpots
                .OrderBy(spot => spot.SpotNumber)
                .ToList();

            IReadOnlyList<DemoStep> steps;

            try
            {
                steps = DemoScenario.Create(demoGarage, configuration);
            }
            catch (InvalidOperationException ex)
            {
                MessageService.ShowError(ex.Message, waitForKey: true);
                return;
            }

            IRenderable CreateDemoFrame(DemoStep step)
            {
                var layout = GetGarageLayout(garageSpots.Count, demoMode: true);

                int selectedIndex = garageSpots.FindIndex(
                    spot => spot.SpotNumber == step.FocusSpot);

                selectedIndex = Math.Max(0, selectedIndex);

                return BuildOverviewLayout(
                    garageSpots,
                    selectedIndex,
                    demoGarage,
                    configuration,
                    layout,
                    lastEvent: step.Description,
                    demoMode: true);
            }

            // Shows the initial empty garage before the first check-in.
            AppTheme.PrepareScreen();

            AnsiConsole.Live(CreateDemoFrame(steps[0]))
                .Start(context =>
                {
                    while (true)
                    {
                        foreach (DemoStep step in steps)
                        {
                            // Execute real domain operations, not UI-only color changes.
                            step.Execute();
                            context.UpdateTarget(CreateDemoFrame(step));

                            int remaining = step.DelayMs;

                            while (remaining > 0)
                            {
                                if (Console.KeyAvailable &&
                                    Console.ReadKey(intercept: true).Key == ConsoleKey.Escape)
                                {
                                    return;
                                }

                                int sleep = Math.Min(25, remaining);
                                Thread.Sleep(sleep);
                                remaining -= sleep;
                            }
                        }

                        // Fresh state on each cycle; no JSON persistence.
                        demoGarage = new ParkingGarage(
                            DemoScenario.DemoParkingSpots);

                        garageSpots = demoGarage.ParkingSpots
                            .OrderBy(spot => spot.SpotNumber)
                            .ToList();

                        steps = DemoScenario.Create(demoGarage, configuration);
                    }
                });
        }

        private static IRenderable BuildOverviewLayout(
            IReadOnlyList<ParkingSpot> garageSpots,
            int selectedSpotIndex,
            ParkingGarage parkingGarage,
            AppConfiguration configuration,
            (int MapWidth, int DetailsWidth, int ColumnsPerRow, int VisibleRows) layout,
            string? lastEvent = null,
            bool demoMode = false)
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
                endVisibleIndex,
                demoMode);

            Panel detailsPanel = BuildDetailsPanel(
                garageSpots[selectedSpotIndex],
                configuration,
                layout.DetailsWidth);

            int totalWidth = layout.MapWidth + 1 + layout.DetailsWidth;
            Panel footer = BuildFooter(
                parkingGarage,
                totalWidth,
                garageSpots[firstVisibleIndex].SpotNumber,
                garageSpots[endVisibleIndex - 1].SpotNumber,
                demoMode);

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


            if (lastEvent is not null)
            {
                Panel lastEventPanel = AppTheme.CreatePanel(
                    new Markup(
                        $"[bold {AppTheme.GoldLight}]" +
                        $"{Markup.Escape(lastEvent)}[/]"),
                    title: "LAST EVENT",
                    width: totalWidth,
                    padding: new Padding(2, 0, 2, 0));

                return new Rows(
                    top,
                    lastEventPanel,
                    footer);
            }

            return new Rows(top, footer);
        }

        private static Grid BuildGarageGrid(
            IReadOnlyList<ParkingSpot> garageSpots,
            int selectedSpotIndex,
            AppConfiguration configuration,
            int columnsPerRow,
            int firstVisibleIndex,
            int endVisibleIndex,
            bool demoMode)
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
                            configuration,
                            demoMode)
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
            int endVisibleIndex,
            bool demoMode)
        {
            Grid map = BuildGarageGrid(
                garageSpots,
                selectedSpotIndex,
                configuration,
                columnsPerRow,
                firstVisibleIndex,
                endVisibleIndex,
                demoMode);

            return AppTheme.CreatePanel(
                map,
                title: demoMode
                ? "LIVE DEMONSTRATION | PARKING MAP"
                : "PARKING MAP",
                width: mapWidth,
                padding: new Padding(1, 0, 1, 0));
        }

        private static Markup CreateParkingSpotCell(
            ParkingSpot spot,
            bool isSelected,
            AppConfiguration configuration,
            bool demoMode)
        {
            // Demo keeps occupancy colors visible even for the focused cell.
            string background = demoMode
                ? GetSpotColor(spot, configuration)
                : isSelected
                    ? AppTheme.SelectedSpotBackground
                    : GetSpotColor(spot, configuration);

            string foreground = demoMode
                ? AppTheme.StatusText
                : isSelected
                    ? AppTheme.SelectedSpotText
                    : AppTheme.StatusText;

            string emphasis = demoMode && isSelected
                ? "bold underline"
                : "bold";

            return new Markup(
                $"[{emphasis} {foreground} on {background}] " +
                $"{spot.SpotNumber:D3} [/] ");
        }

        private static string GetSpotColor(
            ParkingSpot parkingSpot,
            AppConfiguration configuration)
        {
            // Bus reservations always occupy the entire parking spot.
            if (parkingSpot.ReservedByBus is not null)
            {
                return AppTheme.StatusFull;
            }

            int spotCapacity = configuration.Garage!.SpotCapacity;

            Func<IVehicle, int> getSize = vehicle =>
                configuration.VehicleTypes![vehicle.GetType().Name].Size;

            int usedCapacity = parkingSpot.GetUsedCapacity(getSize);

            if (usedCapacity == 0)
            {
                return AppTheme.StatusFree;
            }

            if (usedCapacity >= spotCapacity)
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
            int capacity = configuration.Garage!.SpotCapacity;
            int count = selectedSpot.ParkedVehicles.Count;

            Func<IVehicle, int> getSize = vehicle =>
                configuration.VehicleTypes![vehicle.GetType().Name].Size;

            int used = selectedSpot.ReservedByBus is not null
                ? capacity
                : selectedSpot.GetUsedCapacity(getSize);

            string status = selectedSpot.ReservedByBus is not null && count == 0
                ? "BUS RESERVED"
                : used == 0
                    ? "FREE"
                    : used >= capacity
                        ? "FULL"
                        : "PARTIALLY OCCUPIED";

            string statusColor = used == 0
                ? AppTheme.StatusFree
                : used >= capacity
                    ? AppTheme.StatusFull
                    : AppTheme.StatusPartial;

            var lines = new List<string>
            {
                $"[bold {AppTheme.Gold}]PARKING SPOT {selectedSpot.SpotNumber:D3}[/]",
                "",
                "Status:",
                $"[bold {statusColor}]{status}[/]",
                $"Occupancy: {used} / {capacity}",
                ""
            };

            // Keep panel height constant, including spots shared by four bicycles.
            int textWidth = Math.Max(12, detailsWidth - 4);

            if (selectedSpot.ReservedByBus is Bus reservedBus && count == 0)
            {
                lines.Add($"[bold {AppTheme.GoldLight}]BUS RESERVATION[/]");
                lines.Add("Registration:");
                lines.Add(Markup.Escape(Shorten(reservedBus.Identifier, textWidth)));
            }
            else if (count == 0)
            {
                lines.Add($"[bold {AppTheme.GoldLight}]MAX PER EMPTY SPOT[/]");
                lines.Add($"Car: {capacity / configuration.VehicleTypes!["Car"].Size}");
                lines.Add($"MC: {capacity / configuration.VehicleTypes!["MC"].Size}");
                lines.Add($"Bicycle: {capacity / configuration.VehicleTypes!["Bicycle"].Size}");
            }
            else
            {
                lines.Add($"[bold {AppTheme.GoldLight}]PARKED VEHICLES[/]");

                if (count == 1)
                {
                    Vehicle vehicle = selectedSpot.ParkedVehicles[0];
                    string vehicleText = $"{GetVehicleType(vehicle)} {vehicle.Identifier}";
                    lines.Add(Markup.Escape(Shorten(vehicleText, textWidth)));

                    TimeZoneInfo timeZone = TimeZoneInfo.FindSystemTimeZoneById(
                        configuration.System!.TimeZone!);
                    DateTime localArrival = TimeZoneInfo.ConvertTimeFromUtc(
                        vehicle.ArrivalTime, timeZone);

                    lines.Add($"Date: {localArrival:yyyy-MM-dd}");
                    lines.Add($"Time: {localArrival:HH:mm}");
                    lines.Add($"Duration: {FormatDuration(DateTime.UtcNow - vehicle.ArrivalTime)}");
                }
                else
                {
                    int visible = Math.Min(count, 4);
                    for (int i = 0; i < visible; i++)
                    {
                        Vehicle vehicle = selectedSpot.ParkedVehicles[i];
                        string vehicleText = $"{GetVehicleType(vehicle)} {vehicle.Identifier}";
                        lines.Add(Markup.Escape(Shorten(vehicleText, textWidth)));
                    }

                    if (count > visible)
                    {
                        lines.Add($"+{count - visible} more vehicles");
                    }
                }
            }

            // 12 content rows + 2 border rows, regardless of vehicle count.
            const int maxContentRows = 12;
            if (lines.Count > maxContentRows)
            {
                lines = lines.Take(maxContentRows).ToList();
            }
            while (lines.Count < maxContentRows)
            {
                // Non-breaking space makes the otherwise empty row measurable.
                lines.Add("\u00A0");
            }

            return AppTheme.CreatePanel(
                new Markup(string.Join("\n", lines)),
                title: "SPOT DETAILS",
                width: detailsWidth,
                padding: new Padding(1, 0, 1, 0));
        }

        private static string Shorten(string text, int width)
        {
            return text.Length <= width
                ? text
                : text[..Math.Max(0, width - 3)] + "...";
        }

        private static Panel BuildFooter(
            ParkingGarage parkingGarage,
            int width,
            int firstVisibleSpot,
            int lastVisibleSpot,
            bool demoMode)
        {
            int totalSpots =
                parkingGarage.ParkingSpots.Count;

            int occupiedSpots =
                parkingGarage.ParkingSpots.Count(
                    spot => spot.ParkedVehicles.Count > 0 ||
                    spot.ReservedByBus is not null);

            int freeSpots =
                totalSpots - occupiedSpots;

            int totalVehicles =
                parkingGarage.ParkingSpots.Sum(
                    spot => spot.ParkedVehicles.Count);

            string commands = demoMode
                ? $"[{AppTheme.GoldLight}]ESC[/] " +
                  $"[{AppTheme.Muted}]Stop demonstration[/]"
                : $"[{AppTheme.GoldLight}]← ↑ ↓ →[/] " +
                  $"[{AppTheme.Muted}]Navigate[/]" +
                  $"   [{AppTheme.Muted}]│[/]   " +
                  $"[{AppTheme.GoldLight}]ESC[/] " +
                  $"[{AppTheme.Muted}]Back[/]";

            string content =
                $"[bold {AppTheme.Gold}]FREE SPOTS {freeSpots}[/]" +
                $"   [{AppTheme.Muted}]│[/]   " +
                $"[{AppTheme.Text}]OCCUPIED SPOTS {occupiedSpots}[/]" +
                $"   [{AppTheme.Muted}]│[/]   " +
                $"[{AppTheme.Text}]VEHICLES {totalVehicles}[/]\n\n" +
                commands;

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
                Bicycle => "Bicycle",
                Bus => "Bus",

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