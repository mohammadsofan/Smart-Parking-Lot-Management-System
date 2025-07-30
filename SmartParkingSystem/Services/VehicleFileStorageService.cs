using InventoryApp.Services;
using Serilog;
using SmartParkingSystem.Intefaces;
using SmartParkingSystem.Interfaces;
using SmartParkingSystem.Models;

namespace SmartParkingSystem.Services
{
    internal class VehicleFileStorageService : IVehicleStorageService
    {
        private readonly IFileService<List<Vehicle>> _fileService;
        private readonly string _checkInFilePath, _checkOutFilePath;
        private readonly ILogger _logger = Log.ForContext<VehicleFileStorageService>();

        public VehicleFileStorageService(string checkInFilePath, string checkOutFilePath)
        {
            _checkInFilePath = checkInFilePath;
            _checkOutFilePath = checkOutFilePath;
            _fileService = new FileService<List<Vehicle>>();
        }

        public bool StoreCheckInVehicle(Vehicle vehicle)
        {
            try
            {
                var checkedInVehicles = _fileService.ReadFromFile(_checkInFilePath);
                var id = GetNextVehicleId();
                vehicle.Id = id;
                checkedInVehicles.Add(vehicle);
                _fileService.WriteToFile(checkedInVehicles, _checkInFilePath);

                _logger.Information("Vehicle with ID {Id} checked in at {Time}", vehicle.Id, vehicle.EntryTime);
                return true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to store checked-in vehicle.");
                throw;
            }
        }

        public bool StoreCheckOutVehicle(Vehicle vehicle)
        {
            try
            {
                var checkedInVehicles = _fileService.ReadFromFile(_checkInFilePath);
                var existingCheckedOutVehicle = checkedInVehicles.FirstOrDefault(v => v.Id == vehicle.Id);
                if (existingCheckedOutVehicle == null)
                {
                    _logger.Warning("Vehicle with ID {Id} not found during check-out.", vehicle.Id);
                    return false;
                }

                checkedInVehicles.Remove(existingCheckedOutVehicle);
                _fileService.WriteToFile(checkedInVehicles, _checkInFilePath);

                var checkedOutVehicles = _fileService.ReadFromFile(_checkOutFilePath);
                vehicle.ExitTime = DateTime.Now;
                checkedOutVehicles.Add(vehicle);
                _fileService.WriteToFile(checkedOutVehicles, _checkOutFilePath);

                _logger.Information("Vehicle with ID {Id} checked out at {Time}", vehicle.Id, vehicle.ExitTime);
                return true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to store checked-out vehicle.");
                throw;
            }
        }

        public IList<Vehicle> GetAllCheckedInVehicles()
        {
            try
            {
                var checkedInVehicles = _fileService.ReadFromFile(_checkInFilePath);
                _logger.Information("Retrieved {Count} checked-in vehicles.", checkedInVehicles.Count);
                return checkedInVehicles;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to retrieve checked-in vehicles.");
                throw;
            }
        }

        public IList<Vehicle> GetAllCheckedOutVehicles()
        {
            try
            {
                var checkedOutVehicles = _fileService.ReadFromFile(_checkOutFilePath);
                _logger.Information("Retrieved {Count} checked-out vehicles.", checkedOutVehicles.Count);
                return checkedOutVehicles;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to retrieve checked-out vehicles.");
                throw;
            }
        }

        public long GetNextVehicleId()
        {
            try
            {
                var checkedInVehicles = _fileService.ReadFromFile(_checkInFilePath);
                var checkedOutVehicles = _fileService.ReadFromFile(_checkOutFilePath);

                long currentMaxId = -1;
                if (checkedInVehicles.Count == 0 && checkedOutVehicles.Count == 0)
                {
                    currentMaxId = 0;
                }
                else if (checkedInVehicles.Count == 0)
                {
                    currentMaxId = checkedOutVehicles.Max(v => v.Id);
                }
                else if (checkedOutVehicles.Count == 0)
                {
                    currentMaxId = checkedInVehicles.Max(v => v.Id);
                }
                else
                {
                    currentMaxId = Math.Max(checkedInVehicles.Max(v => v.Id), checkedOutVehicles.Max(v => v.Id));
                }

                _logger.Debug("Next vehicle ID generated: {Id}", currentMaxId + 1);
                return currentMaxId + 1;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to generate next vehicle ID.");
                return -1;
            }
        }
    }
}
