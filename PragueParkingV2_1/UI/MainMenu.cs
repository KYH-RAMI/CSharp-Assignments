using PragueParkingV2_1.Application;
using PragueParkingV2_1.Data.Configuration;
using PragueParkingV2_1.Data.Persistence;
using PragueParkingV2_1.Domain;
using Spectre.Console;
namespace PragueParkingV2_1.UI
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
            ParkingDataService parkingDataService,
            ParkingFeeCalculator feeCalculator,
            ConfigurationService configurationService)
        {
            int selectedIndex = 0;

            try
            {
                while (true)
                {
                    string selectedOption = AppTheme.ShowMenu(
                        title: "PRAGUE PARKING V2.1",
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
                                parkingDataService,
                                feeCalculator);

                            break;
                        
                        case "Move Vehicle":
                            
                            MoveVehicleView.Show(
                                parkingGarage,
                                configuration,
                                parkingDataService);
                            
                            break;
                        
                        case "Find Vehicle":

                            FindVehicleView.Show(
                                parkingGarage,
                                configuration);

                            break;

                        case "Garage Overview":

                            GarageOverView.DisplayGarageOverview(
                                parkingGarage,
                                configuration);

                            break;

                        case "Administration":

                            AdministrationView.Show(
                                configurationService,
                                feeCalculator,
                                () =>
                                {
                                    AppConfiguration newConfiguration =
                                    configurationService.LoadConfiguration(
                                        createIfMissing: false);

                                    var parkingData =
                                        ParkingDataMapper.ToDocument(parkingGarage);

                                    ParkingGarage newGarage =
                                        ParkingDataMapper.ToGarage(
                                            parkingData,
                                            newConfiguration);

                                    var pricing = newConfiguration.Pricing!;

                                    feeCalculator.UpdatePricing(
                                        pricing.CarHourlyPrice,
                                        pricing.MotorcycleHourlyPrice,
                                        pricing.BicycleHourlyPrice,
                                        pricing.BusHourlyPrice,
                                        pricing.FreeMinutes);

                                    parkingGarage = newGarage;
                                    configuration = newConfiguration;
                                });

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