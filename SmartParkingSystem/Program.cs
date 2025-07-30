using Microsoft.Extensions.Configuration;
using Serilog;
using SmartParkingSystem.Dtos;
using SmartParkingSystem.Enums;
using SmartParkingSystem.Intefaces;
using SmartParkingSystem.Models;
using SmartParkingSystem.Services;
using SmartParkingSystem.Utils;

namespace SmartParkingSystem
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var configurations = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configurations)
                .CreateLogger();
            ILogger _logger = Log.ForContext<Program>();
            _logger.Information("Application started");

            try
            {
                var menu = new SmatParkingAppMenu();
                var inputHelper = new ConsoleInputHelper();
                var outputHelper = new ConsoleOutputHelper();
                var notificationService = new FullLotNotificationService();

                string checkInPath = "Data/checkIn.json";
                string checkOutPath = "Data/checkOut.json";

                var parkingLotManager = new ParkingLotManager(5, checkInPath, checkOutPath);
                parkingLotManager.OnLotFull += notificationService.Notify;

                _logger.Information("Initialized parking lot manager with capacity 5");

                int option;
                bool isValidOption;

                do
                {
                    menu.DisplayMenu();
                    do
                    {
                        isValidOption = int.TryParse(Console.ReadLine(), out option);
                        if (!isValidOption || option < 1 || option > 6)
                        {
                            _logger.Warning("Invalid menu option entered: {Option}", option);
                            Console.WriteLine("Please enter a valid option");
                        }
                    }
                    while (!isValidOption || option < 1 || option > 6);

                    var operation = (ConsoleOperationEnum)option;
                    _logger.Information("Selected operation: {Operation}", operation);

                    switch (operation)
                    {
                        case ConsoleOperationEnum.CheckIn:
                            {
                                var vehicle = inputHelper.ReadVehicleInfo();
                                _logger.Information("Attempting to check in vehicle with plate: {Plate}", vehicle.LicensePlate);

                                var result = parkingLotManager.CheckInVehicle(vehicle);
                                _logger.Information("Check-in result: {Message}", result.Message);
                                Console.WriteLine(result.Message);
                                break;
                            }
                        case ConsoleOperationEnum.CheckOut:
                            {
                                var plate = inputHelper.ReadLicensePlate();
                                _logger.Information("Attempting to check out vehicle with plate: {Plate}", plate);

                                var result = parkingLotManager.CheckOutVehicle(plate);
                                _logger.Information("Check-out result: {Message}", result.Message);
                                Console.WriteLine(result.Message);
                                break;
                            }
                        case ConsoleOperationEnum.DisplayCheckedInVehicles:
                            {
                                _logger.Information("Displaying checked-in vehicles");
                                var result = parkingLotManager.GetCheckedInVehicles();
                                if (!result.Succeeded)
                                {
                                    _logger.Warning("Failed to retrieve checked-in vehicles: {Message}", result.Message);
                                    Console.WriteLine(result.Message);
                                }
                                else
                                {
                                    _logger.Information("Retrieved {Count} checked-in vehicles", result.Vehicles.Count);
                                    outputHelper.PrintVehicles(result.Vehicles);
                                }
                                break;
                            }
                        case ConsoleOperationEnum.DisplayCheckedOutVehicles:
                            {
                                _logger.Information("Displaying checked-out vehicles");
                                var result = parkingLotManager.GetCheckedOutVehicles();
                                if (!result.Succeeded)
                                {
                                    _logger.Warning("Failed to retrieve checked-out vehicles: {Message}", result.Message);
                                    Console.WriteLine(result.Message);
                                }
                                else
                                {
                                    _logger.Information("Retrieved {Count} checked-out vehicles", result.Vehicles.Count);
                                    outputHelper.PrintVehicles(result.Vehicles);
                                }
                                break;
                            }
                        case ConsoleOperationEnum.FilterByType:
                            {
                                var selectedType = (VehicleTypeEnum)inputHelper.ReadVehicleType();
                                _logger.Information("Filtering checked-in vehicles by type: {Type}", selectedType);

                                IList<Vehicle> filterdVehicles = new List<Vehicle>();
                                GetVehiclesResultDto result = new GetVehiclesResultDto();

                                if (selectedType == VehicleTypeEnum.Motorcycle)
                                {
                                    result = parkingLotManager.GetCheckedInVehicles(v => v is Motorcycle);
                                }
                                else if (selectedType == VehicleTypeEnum.Car)
                                {
                                    result = parkingLotManager.GetCheckedInVehicles(v => v is Car);
                                }
                                else if (selectedType == VehicleTypeEnum.Truck)
                                {
                                    result = parkingLotManager.GetCheckedInVehicles(v => v is Truck);
                                }

                                if (!result.Succeeded)
                                {
                                    _logger.Warning("Failed to filter vehicles: {Message}", result.Message);
                                    Console.WriteLine(result.Message);
                                }
                                else
                                {
                                    _logger.Information("Filtered and found {Count} vehicles of type {Type}", result.Vehicles.Count, selectedType);
                                    outputHelper.PrintVehicles(result.Vehicles);
                                }

                                break;
                            }
                        case ConsoleOperationEnum.Exit:
                            {
                                _logger.Information("User chose to exit the application");
                                return;
                            }
                    }
                }
                while (option != 6);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Unhandled exception occurred. Application will exit.");
            }
            finally
            {
                _logger.Information("Application shutting down");
                Log.CloseAndFlush();
            }
        }
    }
}
