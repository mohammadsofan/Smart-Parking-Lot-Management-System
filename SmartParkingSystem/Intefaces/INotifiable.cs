using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartParkingSystem.Intefaces
{
    internal interface INotifiable
    {
        void Notify(object sender, EventArgs args);
    }
}
