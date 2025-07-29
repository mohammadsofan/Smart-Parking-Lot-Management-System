using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartParkingSystem.Models
{
    internal class Truck : Vehicle
    {
        public Truck():base() { }
        public Truck(string licensePlate) : base(licensePlate) { }
        public Truck(string licensePlate,DateTime entry) : base(licensePlate, entry) { }
        public override double RatePerHour => 8;

    }
}
