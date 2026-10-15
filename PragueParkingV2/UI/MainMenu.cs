using PragueParkingV2.Data.Configuration;
using PragueParkingV2.Data.Persistence;
using PragueParkingV2.Domain;
using Spectre.Console;
namespace PragueParkingV2.UI
{
    internal static class MainMenu
    {
        private static readonly string[] MenuOptions =
        {
            "Park Vehicle",
            "Check Out Vehicle",
            "Move Vehicle",
            "Find Vehicle",
            "Garage Overview",
            "Administration",
            "Exit"
        };

        public static void Show(
            ParkingGarage parkingGarage,
            AppConfiguration configuration,
            ParkingDataService parkingDataService)
        {
            int selectedIndex = 0;

            try
            {
                while (true)
                {
                    string selectedOption = AppTheme.ShowMenu(
                        title: "PRAGUE PARKING V2",
                        subtitle: "MAIN MENU",
                        options: MenuOptions,
                        selectedIndex: ref selectedIndex,
                        escapeOption: "Exit",
                        description: "PARKING MANAGEMENT SYSTEM",
                        author: "Author: Rami Issa");

                    switch (selectedOption)
                    {
                        case "Park Vehicle":

                            
                            ParkVehicleView.Show(
                                parkingGarage,
                                configuration,
                                parkingDataService);

                            break;
                        
                        case "Check Out Vehicle":

                            CheckOutVehicleView.Show(
                                parkingGarage,
                                configuration,
                                parkingDataService);

                            break;
                        
                        case "Move Vehicle":
                            
                            MoveVehicleView.Show(
                                parkingGarage,
                                configuration,
                                parkingDataService);
                            
                            break;

                        case "Garage Overview":

                            GarageOverView.DisplayGarageOverview(
                                parkingGarage,
                                configuration);

                            break;

                        case "Exit":
                            return;

                        default:

                            AppTheme.PrepareScreen();

                            MessageService.ShowInfo(
                                "This feature is under development.");

                            AnsiConsole.MarkupLine(
                                "\n[white]Press any key to continue...[/]");

                            Console.ReadKey(intercept: true);

                            break;
                    }
                }
            }
            finally
            {
                AppTheme.RestoreScreen();
            }
        }
    }
}