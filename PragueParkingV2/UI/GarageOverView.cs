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
                AnsiConsole.Clear();

                AnsiConsole.MarkupLine(
                    "[bold cyan]GARAGE OVERVIEW[/]");

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

                var detailsPanel = new Panel(
                    new Markup(
                        $"[bold]Spot:[/] {selectedSpot.SpotNumber:D3}\n" +
                        $"[bold]Vehicle ID:[/] {selectedSpot.ParkedVehicles.Count}"))
                {
                    Header = new PanelHeader(" SPOT DETAILS "),
                    Border = BoxBorder.Rounded,
                    Width = 35
                };

                var columns = new Columns(grid, detailsPanel)
                {
                    Expand = false,
                    Padding = new Padding(1, 0, 1, 0)
                };

                AnsiConsole.Write(columns);

                AnsiConsole.WriteLine();

                AnsiConsole.MarkupLine(
                    $"[bold Red]PAGE {currentPage:00} / {totalPages:00}[/]  " +
                    $"[grey]|[/]  " +
                    $"[white]SPOTS {pageSpots[0].SpotNumber:D3}-{pageSpots[^1].SpotNumber:D3}[/]  " +
                    $"[grey]|[/]  " +
                    $"[white]TOTAL {totalSpots}[/]");

                AnsiConsole.MarkupLine(
                    "[grey]Navigate:[/] [white]Arrow Keys[/]   " +
                    "[grey]|[/]   " +
                    "[grey]Pages:[/] [white]PgUp / PgDn[/]   " +
                    "[grey]|[/]   " +
                    "[grey]Exit:[/] [white]ESC[/]");

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
    }
}
