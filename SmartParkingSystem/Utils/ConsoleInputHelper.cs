using SmartParkingSystem.Enums;
using SmartParkingSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartParkingSystem.Utils
{
    internal class ConsoleInputHelper
    {
        public int ReadVehicleType()
        {
            Console.WriteLine("Choose one of the following vehicle types (a/b/c):");
            Console.WriteLine("""
                1. Motorcycle ($3 per hour)
                2. Car ($5 per hour)
                3. Truck (8$ per hour)
                """);
            var isValid = int.TryParse(Console.ReadLine(), out int option);
            while (!isValid || option < 1 || option > 3)
            {
                Console.WriteLine("Please enter a valid option (1/2/3)");
                isValid = int.TryParse(Console.ReadLine(), out option);
            }
            return option;
        }
        public string ReadLicensePlate()
        {
            Console.WriteLine("Enter vehicle license Plate:");
            var licensePlate = Console.ReadLine() ?? string.Empty;
            while (licensePlate.Length < 1)
            {
                Console.WriteLine("Please enter a valid license Plate");
                licensePlate = Console.ReadLine() ?? string.Empty;
            }
            return licensePlate;
        }
        public Vehicle ReadVehicleInfo()
        {

            var option = ReadVehicleType();
            var licensePlate = ReadLicensePlate();
            Vehicle vehicle;
            var operation = (VehicleTypeEnum) option;
            if (operation == VehicleTypeEnum.Motorcycle)
            {
                vehicle = new Motorcycle(licensePlate);
            }
            else if (operation == VehicleTypeEnum.Car)
            {
                vehicle = new Car(licensePlate);
            }
            else
            {
                vehicle = new Truck(licensePlate);
            }

            vehicle.EntryTime = DateTime.Now;

            return vehicle;
        }
    }
}
