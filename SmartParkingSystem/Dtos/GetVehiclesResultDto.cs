using SmartParkingSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartParkingSystem.Dtos
{
    internal class GetVehiclesResultDto:ParkingLotResultDto
    {
        public IList<Vehicle> Vehicles { get; set; } = new List<Vehicle>();
    }
}
