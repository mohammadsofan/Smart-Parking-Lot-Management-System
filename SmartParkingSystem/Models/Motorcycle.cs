using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartParkingSystem.Models
{
    internal class Motorcycle:Vehicle
    {
        public Motorcycle():base() { }
        public Motorcycle(string licensePlate) : base(licensePlate) { }
        public Motorcycle(string licensePlate,DateTime entry) : base(licensePlate, entry) { }
        public override double RatePerHour => 3;
    }
}
