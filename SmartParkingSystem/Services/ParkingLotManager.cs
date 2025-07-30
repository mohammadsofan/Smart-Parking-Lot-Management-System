using Serilog;
using SmartParkingSystem.CustomeExceptions;
using SmartParkingSystem.Dtos;
using SmartParkingSystem.Events;
using SmartParkingSystem.Intefaces;
using SmartParkingSystem.Models;
using SmartParkingSystem.Validators;
using System.Linq.Expressions;

namespace SmartParkingSystem.Services
{
    internal class ParkingLotManager
    {
        private readonly IVehicleStorageService _vehicleStorageService;
        private readonly int _totalSpots;
        private readonly VehicleValidator _vehicelValidator;
        private readonly ILogger _logger = Log.ForContext<ParkingLotManager>();

        public delegate void LotFullHandler(object sender, EventArgs e);
        public event LotFullHandler? OnLotFull;

        public ParkingLotManager(
            int totalSpots,
            string checkInFilePath,
            string checkOutFilePath
            )
        {
            _vehicleStorageService = new VehicleFileStorageService(checkInFilePath, checkOutFilePath); 
            _vehicelValidator = new VehicleValidator();
            _totalSpots = totalSpots;
        }

        public ParkingLotResultDto CheckInVehicle(Vehicle vehicle)
        {
            try
            {
                _logger.Information("Attempting to check-in vehicle with plate: {Plate}", vehicle.LicensePlate);

                var validationResult = _vehicelValidator.IsValid(vehicle);
                if (!validationResult.IsValid)
                {
                    string errors = string.Join(Environment.NewLine, validationResult.Errors.Select(e => e.Message));
                    _logger.Warning("Validation failed for vehicle {Plate}: {Errors}", vehicle.LicensePlate, errors);
                    throw new ParkingLotException($"Validation error: {errors}");
                }

                var checkedInVehicles = _vehicleStorageService.GetAllCheckedInVehicles();
                if (checkedInVehicles.Any(v => v.LicensePlate == vehicle.LicensePlate))
                {
                    _logger.Warning("Duplicate check-in attempt for vehicle {Plate}", vehicle.LicensePlate);
                    throw new ParkingLotException($"Duplicate entry: Vehicle with plate '{vehicle.LicensePlate}' is already checked in.");
                }

                var currentUsedSpots = checkedInVehicles.Count;
                if (currentUsedSpots >= _totalSpots)
                {
                    _logger.Warning("Parking full. Cannot check-in vehicle {Plate}", vehicle.LicensePlate);
                    throw new ParkingLotException("Parking slots are full. Cannot add a new vehicle.");
                }

                _vehicleStorageService.StoreCheckInVehicle(vehicle);
                _logger.Information("Vehicle {Plate} checked in successfully", vehicle.LicensePlate);

                if (currentUsedSpots + 1 == _totalSpots)
                {
                    _logger.Warning("Parking lot has reached full capacity with last check-in: {Plate}", vehicle.LicensePlate);
                    OnLotFull?.Invoke(this, new FullLotEventArgs
                    {
                        Message = "The last available spot has been filled. The parking lot is now full."
                    });
                }

                return new ParkingLotResultDto
                {
                    Succeeded = true,
                    Message = "Vehicle checked in successfully!"
                };
            }
            catch (ParkingLotException ex)
            {
                _logger.Error(ex, "Parking lot error during check-in for plate {Plate}", vehicle.LicensePlate);
                return new ParkingLotResultDto
                {
                    Succeeded = false,
                    Message = ex.Message
                };
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Unexpected error during check-in for plate {Plate}", vehicle.LicensePlate);
                return new ParkingLotResultDto
                {
                    Succeeded = false,
                    Message = "Unexpected error occurred."
                };
            }
        }

        public ParkingLotResultDto CheckOutVehicle(string licensePlate)
        {
            try
            {
                _logger.Information("Attempting to check-out vehicle with plate: {Plate}", licensePlate);

                var checkedInVehicles = _vehicleStorageService.GetAllCheckedInVehicles();
                var vehicle = checkedInVehicles.FirstOrDefault(v => v.LicensePlate == licensePlate);
                if (vehicle is null)
                {
                    _logger.Warning("Check-out failed. No vehicle found with plate: {Plate}", licensePlate);
                    throw new ParkingLotException("Unknown license plate.");
                }

                _vehicleStorageService.StoreCheckOutVehicle(vehicle);
                _logger.Information("Vehicle {Plate} checked out successfully", licensePlate);

                return new ParkingLotResultDto
                {
                    Succeeded = true,
                    Message = "Vehicle checked out successfully!"
                };
            }
            catch (ParkingLotException ex)
            {
                _logger.Error(ex, "Parking lot error during check-out for plate {Plate}", licensePlate);
                return new ParkingLotResultDto
                {
                    Succeeded = false,
                    Message = ex.Message
                };
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Unexpected error during check-out for plate {Plate}", licensePlate);
                return new ParkingLotResultDto
                {
                    Succeeded = false,
                    Message = "Unexpected error occurred."
                };
            }
        }

        public GetVehiclesResultDto GetCheckedInVehicles(Expression<Func<Vehicle, bool>>? filter = null)
        {
            try
            {
                var vehicles = _vehicleStorageService.GetAllCheckedInVehicles().AsQueryable();
                var filtered = filter is null ? vehicles.ToList() : vehicles.Where(filter).ToList();
                filtered = filtered.OrderByDescending(v => v.EntryTime).ToList();

                _logger.Information("Retrieved {Count} checked-in vehicles", filtered.Count);

                return new GetVehiclesResultDto
                {
                    Succeeded = true,
                    Message = "Checked-in vehicles retrieved successfully!",
                    Vehicles = filtered
                };
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to retrieve checked-in vehicles.");
                return new GetVehiclesResultDto
                {
                    Succeeded = false,
                    Message = "Unexpected error occurred."
                };
            }
        }

        public GetVehiclesResultDto GetCheckedOutVehicles(Expression<Func<Vehicle, bool>>? filter = null)
        {
            try
            {
                var vehicles = _vehicleStorageService.GetAllCheckedOutVehicles().AsQueryable();
                var filtered = filter is null ? vehicles.ToList() : vehicles.Where(filter).ToList();
                filtered = filtered.OrderByDescending(v => v.ExitTime).ToList();

                _logger.Information("Retrieved {Count} checked-out vehicles", filtered.Count);

                return new GetVehiclesResultDto
                {
                    Succeeded = true,
                    Message = "Checked-out vehicles retrieved successfully!",
                    Vehicles = filtered
                };
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to retrieve checked-out vehicles.");
                return new GetVehiclesResultDto
                {
                    Succeeded = false,
                    Message = "Unexpected error occurred."
                };
            }
        }
    }
}
