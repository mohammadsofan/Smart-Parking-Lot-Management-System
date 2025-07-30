using SmartParkingSystem.Models;


namespace SmartParkingSystem.Intefaces
{
    internal interface IVehicleStorageService
    {
        bool StoreCheckInVehicle(Vehicle vehicle);
        bool StoreCheckOutVehicle(Vehicle vehicle);
        IList<Vehicle> GetAllCheckedInVehicles();
        IList<Vehicle> GetAllCheckedOutVehicles();
    }
}
