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
            var menu = new SmatParkingAppMenu();
            var inputHelper = new ConsoleInputHelper();
            var outputHelper = new ConsoleOutputHelper();
            var notificationService = new FullLotNotificationService();

            string checkInPath = "Data/checkIn.json";
            string checkOutPath = "Data/checkOut.json";
            var parkingLotManager = new ParkingLotManager(5, checkInPath, checkOutPath);
            parkingLotManager.OnLotFull += notificationService.Notify;

            int option;
            bool isValidOption;

            do {
                menu.DisplayMenu();
                do
                {
                    isValidOption = int.TryParse(Console.ReadLine(), out option);
                    if (!isValidOption || option < 1 || option > 6)
                        Console.WriteLine("Please enter a valid option");
                } 
                while (!isValidOption || option < 1 || option > 6);
                var operation = (ConsoleOperationEnum)option;
                switch (operation)
                {
                    case ConsoleOperationEnum.CheckIn:
                        {
                            var result = parkingLotManager.CheckInVehicle(inputHelper.ReadVehicleInfo());
                            Console.WriteLine(result.Message);
                            break;
                        }
                    case ConsoleOperationEnum.CheckOut:
                        {
                            var result = parkingLotManager.CheckOutVehicle(inputHelper.ReadLicensePlate());
                            Console.WriteLine(result.Message);
                            break;
                        }
                    case ConsoleOperationEnum.DisplayCheckedInVehicles:
                        {
                            var result = parkingLotManager.GetCheckedInVehicles();
                            if (!result.Succeeded)
                            {
                                Console.WriteLine(result.Message);
                            }
                            else {
                                outputHelper.PrintVehicles(result.Vehicles);
                            }
                            break;
                        }
                    case ConsoleOperationEnum.DisplayCheckedOutVehicles:
                        {
                            var result = parkingLotManager.GetCheckedOutVehicles();
                            if (!result.Succeeded)
                            {
                                Console.WriteLine(result.Message);
                            }
                            else
                            {
                                outputHelper.PrintVehicles(result.Vehicles);
                            }
                            break;
                        }
                    case ConsoleOperationEnum.FilterByType:
                        {
                            var selectedType = (VehicleTypeEnum) inputHelper.ReadVehicleType();
                            IList<Vehicle> filterdVehicles = new List<Vehicle>();
                            GetVehiclesResultDto result =new GetVehiclesResultDto();
                            if(selectedType == VehicleTypeEnum.Motorcycle)
                            {
                                result = parkingLotManager.GetCheckedInVehicles(v => v is Motorcycle);
                            }
                            else if(selectedType == VehicleTypeEnum.Car)
                            {
                                result = parkingLotManager.GetCheckedInVehicles(v => v is Car);
                            }
                            else if(selectedType == VehicleTypeEnum.Truck)
                            {
                                result = parkingLotManager.GetCheckedInVehicles(v => v is Truck);
                            }
                            if (!result.Succeeded)
                            {
                                Console.WriteLine(result.Message);
                            }
                            else
                            {
                                outputHelper.PrintVehicles(result.Vehicles);
                            }

                            break;
                        }
                    case ConsoleOperationEnum.Exit:
                        {
                            return;
                        }
                }
            }
            while (option != 6);
        }

    }
}
