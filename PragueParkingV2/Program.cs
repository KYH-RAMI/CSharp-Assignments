using PragueParkingV2.Data.Configuration;
using PragueParkingV2.Domain;
using PragueParkingV2.UI;
using System.IO;
using System.Runtime.ConstrainedExecution;
using System.Text.Json;

var configurationService = new ConfigurationService();


try
{
    var configuration = configurationService.LoadConfiguration();

    var parkingGarage = new ParkingGarage(
        configuration.Garage!.NumberOfParkingSpots);

    var pricing = configuration.Pricing!;

    var feeCalculator = new ParkingFeeCalculator(
        pricing.CarHourlyPrice,
        pricing.MotorcycleHourlyPrice,
        pricing.FreeMinutes);

    MessageService.ShowSuccess(
        $"Parking garage created with {parkingGarage.ParkingSpots.Count} parking spots.");

    MessageService.ShowInfo(
        $"Time zone: {configuration.System!.TimeZone}");


}


catch (InvalidDataException ex)
{
    MessageService.ShowError(
        $"Invalid configuration: {ex.Message}");

    Environment.ExitCode = 1;
    return;
}
catch (JsonException)
{
    MessageService.ShowError(
        "The configuration file contains invalid JSON.");

    Environment.ExitCode = 1;
    return;
}
catch (UnauthorizedAccessException)
{
    MessageService.ShowError(
        "The application does not have permission to access the configuration file.");

    Environment.ExitCode = 1;
    return;
}
catch (IOException)
{
    MessageService.ShowError(
        "The configuration file could not be read.");

    Environment.ExitCode = 1;
    return;
}
catch (Exception)
{
    MessageService.ShowError(
        "An unexpected error occurred. The application cannot continue safely.");

    Environment.ExitCode = 1;
    return;
}

