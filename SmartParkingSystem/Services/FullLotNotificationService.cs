using SmartParkingSystem.Events;
using SmartParkingSystem.Intefaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartParkingSystem.Services
{
    internal class FullLotNotificationService : INotifiable
    {
        public void Notify(object sender, EventArgs args)
        {
            var fullLotEvent = args as FullLotEventArgs;
            Console.WriteLine(fullLotEvent?.Message);
        }
    }
}
