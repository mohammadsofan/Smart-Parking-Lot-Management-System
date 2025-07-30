using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartParkingSystem.Models
{
    internal class Car:Vehicle
    {
        public Car():base() { }
        public Car(string licensePlate) : base(licensePlate) { }
        public Car(string licensePlate, DateTime entry) : base(licensePlate, entry) { }
        public override double RatePerHour => 5;
    }
}
