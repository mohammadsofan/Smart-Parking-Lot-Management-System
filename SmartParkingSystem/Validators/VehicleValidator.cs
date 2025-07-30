using SmartParkingSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartParkingSystem.Validators
{
    internal class ValidationResult
    {
        public bool IsValid { get; set; }
        public IList<Error> Errors { get; set; } = new List<Error>();
    }
    internal class Error
    {
        public string Field {  get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
    }
    internal class VehicleValidator
    {
        public ValidationResult IsValid(Vehicle vehicle)
        {
            List<Error> errors = new List<Error>();
            if(vehicle is null)
            {
                return new ValidationResult { IsValid = false };
            }
            if(vehicle.LicensePlate.Length < 5)
            {
                errors.Add(new Error()
                {
                    Field = "LicensePlate",
                    Message = "License plate must be at least 5 characters long."
                });
            }
            return new ValidationResult()
            {
                IsValid = errors.Count == 0,
                Errors = errors
            };
        }
    }
}
