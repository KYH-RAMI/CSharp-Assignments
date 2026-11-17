using PragueParkingV2_1.Data.Configuration;
using PragueParkingV2_1.Domain;
using Spectre.Console;
using System.Text.Json;

namespace PragueParkingV2_1.UI
{
    internal static class AdministrationView
    {
        public static void Show(
            ConfigurationService configurationService,
            ParkingFeeCalculator feeCalculator,
            Action reloadConfiguration)
        {
            int selectedIndex = 0;

            string[] menuOptions =
            {
                "Reload Pricing",
                "Reload Configuration",
                "Back to Main Menu"
            };

            while (true)
            {
                string selectedOption = AppTheme.ShowMenu(
                    title: "PRAGUE PARKING V2.1",
                    subtitle: "ADMINISTRATION",
                    options: menuOptions,
                    selectedIndex: ref selectedIndex,
                    escapeOption: "Back to Main Menu",
                    description: "PARKING MANAGEMENT SYSTEM",
                    author: "Author: Rami Issa");

                switch (selectedOption)
                {
                    case "Reload Pricing":

                        AppTheme.PrepareScreen();

                        try
                        {
                            var pricing = configurationService.LoadPricing();

                            feeCalculator.UpdatePricing(
                                pricing.CarHourlyPrice,
                                pricing.MotorcycleHourlyPrice,
                                pricing.BicycleHourlyPrice,
                                pricing.BusHourlyPrice,
                                pricing.FreeMinutes);

                            MessageService.ShowSuccess(
                                "Pricing reloaded successfully!\n\n" +
                                $"Car: {pricing.CarHourlyPrice} CZK/hour\n" +
                                $"MC: {pricing.MotorcycleHourlyPrice} CZK/hour\n" +
                                $"Bicycle: {pricing.BicycleHourlyPrice} CZK/hour\n" +
                                $"Bus: {pricing.BusHourlyPrice} CZK/hour\n" +
                                $"Free minutes: {pricing.FreeMinutes}");
                        }

                        catch (Exception ex) when (
                        ex is IOException or
                        UnauthorizedAccessException or
                        JsonException or
                        InvalidDataException or
                        ArgumentOutOfRangeException)
                        {
                            MessageService.ShowError(
                                $"Failed to reload pricing:\n{Markup.Escape(ex.Message)}");
                        }

                        AnsiConsole.WriteLine();

                        int leftMargin = Math.Max(0, (Console.WindowWidth - 62) / 2);

                        AnsiConsole.MarkupLine(
                            $"{new string(' ', leftMargin)}[{AppTheme.Muted}]Press any key to continue...[/]");

                        Console.ReadKey(intercept: true);

                        break;
                    
                    case "Reload Configuration":
                        {
                            AppTheme.PrepareScreen();

                            try
                            {
                                reloadConfiguration();

                                MessageService.ShowSuccess(
                                    "Configuration reloaded successfully!");
                            }
                            catch (Exception ex) when (
                                ex is IOException or
                                UnauthorizedAccessException or
                                JsonException or
                                InvalidDataException or
                                InvalidOperationException or
                                ArgumentOutOfRangeException)
                            {
                                MessageService.ShowError(
                                    "Failed to reload configuration:\n" +
                                    Markup.Escape(ex.Message));
                            }

                            AnsiConsole.WriteLine();

                             leftMargin = Math.Max(
                                0,
                                (Console.WindowWidth - AppTheme.FormWidth) / 2);

                            AnsiConsole.MarkupLine(
                                $"{new string(' ', leftMargin)}" +
                                $"[{AppTheme.Muted}]Press any key to continue...[/]");

                            Console.ReadKey(intercept: true);

                            break;
                        }
                    case "Back to Main Menu":
                        return;
                }
            }
        }
    }
}