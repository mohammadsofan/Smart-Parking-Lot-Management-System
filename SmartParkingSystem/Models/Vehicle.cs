using SmartParkingSystem.Intefaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SmartParkingSystem.Models
{
    [JsonPolymorphic(TypeDiscriminatorPropertyName = "Type")]
    [JsonDerivedType(typeof(Car), "Car")]
    [JsonDerivedType(typeof(Truck), "Truck")]
    [JsonDerivedType(typeof(Motorcycle), "Motorcycle")]
    internal class Vehicle : IBillable
    {
        public long Id {  get; set; }
        public string LicensePlate { get; set; } = string.Empty;
        public DateTime EntryTime { get; set; }
        public DateTime ExitTime { get; set; }
        public virtual double RatePerHour => 2;
        public Vehicle()
        {
            Id = 0;
            LicensePlate = string.Empty;
            EntryTime = DateTime.MinValue;
            ExitTime = DateTime.MinValue;
        }
        public Vehicle(string licensePlate)
        {
            LicensePlate = licensePlate;
            EntryTime = DateTime.Now;
            ExitTime = DateTime.MinValue;
        }
        public Vehicle(string licensePlate, DateTime entryTime)

        {
            LicensePlate = licensePlate;
            EntryTime = entryTime;
            ExitTime = DateTime.MinValue;
        }

        public override string ToString()
        {
            (double hours, double fee) = CalculateFee();
            if (ExitTime == DateTime.MinValue)
            {
                return $"""
                    Id: {Id},
                    Type: {GetType().Name},
                    Plate: {LicensePlate},
                    Entry: {EntryTime.ToString("yyyy-MM-dd  hh:mm tt")},
                    Elapsed Hours : {hours} 
                    Total Fee: ${fee}
                    """;
            }
            else
            {
                return $"""
                    Id: {Id},
                    Type: {GetType().Name},
                    Plate: {LicensePlate},
                    Entry: {EntryTime.ToString("yyyy-MM-dd  hh:mm tt")},
                    Exit: {ExitTime.ToString("yyyy-MM-dd  hh:mm tt")},
                    Total hours: {hours} 
                    Total Fee: ${fee}
                    """;
            }
        }

        public virtual (double hours, double fee) CalculateFee()
        {
            double hours,fee;
            TimeSpan duration;
            if (ExitTime == DateTime.MinValue)
            {
                duration = DateTime.Now - EntryTime;
            }
            else
            {
                duration = ExitTime - EntryTime;
            }
            hours = Math.Round(duration.TotalHours, 3);
            fee = Math.Round(hours * RatePerHour, 3);
            return (hours, fee);

        }
    }
}
