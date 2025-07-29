using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace SmartParkingSystem.CustomeExceptions
{
    internal class ParkingLotException:Exception
    {
        public ParkingLotException(string message) : base(message) { }
        public ParkingLotException(string message,Exception innerException) : base(message, innerException) { }
    }
}
