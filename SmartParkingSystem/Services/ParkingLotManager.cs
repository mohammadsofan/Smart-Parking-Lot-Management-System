using SmartParkingSystem.CustomeExceptions;
using SmartParkingSystem.Dtos;
using SmartParkingSystem.Events;
using SmartParkingSystem.Intefaces;
using SmartParkingSystem.Models;
using SmartParkingSystem.Validators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SmartParkingSystem.Services
{
    internal class ParkingLotManager
    {

        private readonly IVehicleStorageService _vehicleStorageService;
        private readonly int _totalSpots;
        private readonly VehicelValidator _vehicelValidator;
        public delegate void LotFullHandler(object sender, EventArgs e);
        public event LotFullHandler? OnLotFull;
        public ParkingLotManager(int totalSpots, string checkInFilePath, string checkOutFilePath)
        {
            _vehicleStorageService = new VehicleFileStorageService(checkInFilePath, checkOutFilePath);
            _vehicelValidator = new VehicelValidator();
            _totalSpots = totalSpots;
        }
        public ParkingLotResultDto CheckInVehicle(Vehicle vehicle)
        {
            try
            {
                var validationResult = _vehicelValidator.IsValid(vehicle);
                if (!validationResult.IsValid)
                {
                    string errors = string.Join(Environment.NewLine, validationResult.Errors.Select(e => e.Message));
                    throw new ParkingLotException($"Fail due to validation error : {errors}");
                }
                var checkedInVehicles = _vehicleStorageService.GetAllCheckedInVehicles();
                var isDuplicatePlate = checkedInVehicles.Any(v => v.LicensePlate == vehicle.LicensePlate);
                if (isDuplicatePlate)
                {
                    throw new ParkingLotException($"Duplicate entry: A vehicle with license plate '{vehicle.LicensePlate}' already exists in the parking lot.");
                }
                var currentUsedSpots = checkedInVehicles.Count;
                if (currentUsedSpots >= _totalSpots)
                {
                    throw new ParkingLotException("Parking slots are full. Cannot add a new vehicle.");
                }
                else
                {
                    _vehicleStorageService.StoreCheckInVehicle(vehicle);
                    if (currentUsedSpots + 1 == _totalSpots)
                    {

                        OnLotFull?.Invoke(this, new FullLotEventArgs()
                        {
                            Message= "The last available spot has been filled. The parking lot is now at full capacity."
                        });
                    }
                    return new ParkingLotResultDto()
                    {
                        Succeeded = true,
                        Message="Vehicle checked in successfully!"
                    };
                }
            }
            catch (Exception ex) when (ex is ParkingLotException)
            {
                return new ParkingLotResultDto()
                {
                    Succeeded = false,
                    Message = ex.Message
                };
            }
            catch (Exception)
            {
                return new ParkingLotResultDto()
                {
                    Succeeded = false,
                    Message = "Unexpected error occured."
                };
            }
        }
        public ParkingLotResultDto CheckOutVehicle(string lecinsePlate)
        {
            try
            {
                var checkedInVehicles = _vehicleStorageService.GetAllCheckedInVehicles();
                var vehicle = checkedInVehicles.FirstOrDefault(v => v.LicensePlate == lecinsePlate);
                if (vehicle is null)
                {
                    throw new ParkingLotException("Unknown license plate.");
                }
                _vehicleStorageService.StoreCheckOutVehicle(vehicle);
                return new ParkingLotResultDto()
                {
                    Succeeded = true,
                    Message = "Vehicle checked out successfully!"
                };
            }
            catch (Exception ex) when (ex is ParkingLotException)
            {
                return new ParkingLotResultDto()
                {
                    Succeeded = false,
                    Message = ex.Message
                };
            }
            catch (Exception)
            {
                return new ParkingLotResultDto()
                {
                    Succeeded = false,
                    Message = "Unexpected error occured."
                };
            }
        }
        public GetVehiclesResultDto GetCheckedInVehicles(Expression<Func<Vehicle,bool>>? filter = null)
        {
            try
            {
                var vehicles = _vehicleStorageService.GetAllCheckedInVehicles().AsQueryable();
                var filterdVehicles = filter is null ? vehicles.ToList() : vehicles.Where(filter).ToList();
                filterdVehicles = filterdVehicles.OrderByDescending(v => v.EntryTime).ToList();
                return new GetVehiclesResultDto()
                {
                    Succeeded = true,
                    Message = "Checked in vehicles retrived successfully!",
                    Vehicles = filterdVehicles
                };
            }
            catch (Exception ex)
            {
                return new GetVehiclesResultDto()
                {
                    Succeeded = false,
                    Message = $"Unexpected error occured. {ex.Message}",
                };
            }
        }
        public GetVehiclesResultDto GetCheckedOutVehicles(Expression<Func<Vehicle, bool>>? filter = null)
        {
            try
            {
                var vehicles = _vehicleStorageService.GetAllCheckedOutVehicles().AsQueryable();
                var filterdVehicles = filter is null ? vehicles.ToList() : vehicles.Where(filter).ToList();
                filterdVehicles = filterdVehicles.OrderByDescending(v => v.ExitTime).ToList();
                return new GetVehiclesResultDto()
                {
                    Succeeded = true,
                    Message = "Checked out vehicles retrived successfully!",
                    Vehicles = filterdVehicles
                };
            }
            catch (Exception)
            {
                return new GetVehiclesResultDto()
                {
                    Succeeded = false,
                    Message = "Unexpected error occured.",
                };
            }
        }
    }
}