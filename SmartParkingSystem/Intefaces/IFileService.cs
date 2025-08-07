
namespace SmartParkingSystem.Interfaces
{
    internal interface IFileService<T>
    {
        void WriteToFile(T value,string filePath);
        T ReadFromFile(string filePath);
    }
}
