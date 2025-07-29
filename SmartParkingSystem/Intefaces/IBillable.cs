using SmartParkingSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartParkingSystem.Intefaces
{
    internal interface IBillable
    {
        (double hours, double fee) CalculateFee();
    }
}
