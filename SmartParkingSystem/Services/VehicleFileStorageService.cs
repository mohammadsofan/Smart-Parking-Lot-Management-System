using InventoryApp.Services;
using SmartParkingSystem.Intefaces;
using SmartParkingSystem.Interfaces;
using SmartParkingSystem.Models;
namespace SmartParkingSystem.Services
{
    internal class VehicleFileStorageService : IVehicleStorageService
    {
        private readonly IFileService<List<Vehicle>> _fileService;
        private readonly string _checkInFilePath,_checkOutFilePath;
        public VehicleFileStorageService(string checkInFilePath,string checkOutFilePath)
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
                return true;

            }
            catch (Exception)
            {
                throw;
            }
            
        }

        public bool StoreCheckOutVehicle(Vehicle vehicle)
        {
            try
            {
                var checkedInVehicles = _fileService.ReadFromFile(_checkInFilePath);
                var existingCheckedOutVehicle = checkedInVehicles.FirstOrDefault(v => v.Id == vehicle.Id);
                if(existingCheckedOutVehicle == null)
                {
                    return false;
                }
                checkedInVehicles.Remove(existingCheckedOutVehicle);
                _fileService.WriteToFile(checkedInVehicles, _checkInFilePath);
                var checkedOutVehicles = _fileService.ReadFromFile(_checkOutFilePath);
                vehicle.ExitTime = DateTime.Now;
                checkedOutVehicles.Add(vehicle);
                _fileService.WriteToFile(checkedOutVehicles, _checkOutFilePath);
                return true;

            }
            catch(Exception)
            {
                throw;
            }
        }
        public IList<Vehicle> GetAllCheckedInVehicles()
        {
            try
            {
                var checkedInVehicles = _fileService.ReadFromFile(_checkInFilePath);
                return checkedInVehicles;
            }
            catch (Exception)
            {
                throw;
            }

        }
        public IList<Vehicle> GetAllCheckedOutVehicles()
        {
            try
            {
                var checkedOutVehicles = _fileService.ReadFromFile(_checkOutFilePath);
                return checkedOutVehicles;
            }
            catch (Exception)
            {
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
                else if(checkedOutVehicles.Count == 0)
                {
                    currentMaxId = checkedInVehicles.Max(v => v.Id);
                }
                else
                {
                    currentMaxId = Math.Max(checkedInVehicles.Max(v => v.Id), checkedOutVehicles.Max(v => v.Id));
                }
                return currentMaxId + 1;
            }
            catch (Exception)
            {
                return -1;
            }
        }
    }
}
