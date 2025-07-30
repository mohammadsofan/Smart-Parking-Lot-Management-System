using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartParkingSystem.Utils
{
    internal class SmatParkingAppMenu
    {
        public void DisplayMenu()
        {
            Console.WriteLine("""
                ===================== Menu =======================
                Please Choose one of the follwoing options:
                1. Check-In Vehicle
                2. Check-Out Vehicle
                3. View Parked Vehicles
                4. View checked out Vehicles
                5. Filter by Vehicle Type
                6. Exit
                ==================================================
                """);
        }
    }
}
