using SmartParkingSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartParkingSystem.Utils
{
    internal class ConsoleOutputHelper
    {
        public void PrintVehicle(Vehicle vehicle)
        {
            Console.WriteLine("=========== Vehicle Info ===========");
            Console.WriteLine(vehicle);
            Console.WriteLine("===================================");
        }
        public void PrintVehicles(IList<Vehicle> checkedInVehicles)
        {
            foreach (var vehicle in checkedInVehicles)
            {
                PrintVehicle(vehicle);
            }
            Console.WriteLine($"Total vehicles: {checkedInVehicles.Count}");
        }
    }
}
