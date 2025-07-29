using SmartParkingSystem.Interfaces;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

namespace InventoryApp.Services
{
    internal class FileService<T> : IFileService<T>
    {
        private void CreateDirectory(string path)
        {
            var directoryName = Path.GetDirectoryName(path);
            if (directoryName is not null && !Directory.Exists(directoryName))
            {
                Directory.CreateDirectory(directoryName);
            }
        }
        public void WriteToFile(T value, string filePath)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(filePath))
                {
                    throw new ArgumentException("File path must not be null or empty.", nameof(filePath));
                }
                CreateDirectory(filePath);
                var json = JsonSerializer.Serialize(value,new JsonSerializerOptions { WriteIndented = true});
                using FileStream fileStream = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
                using StreamWriter writer = new StreamWriter(fileStream);

                fileStream.SetLength(0); 
                fileStream.Position = 0;
                writer.Write(json);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public T ReadFromFile(string filePath)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(filePath))
                {
                    throw new ArgumentException("File path must not be null or empty.", nameof(filePath));
                }
                CreateDirectory(filePath);
                using FileStream fileStream = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
                using StreamReader reader = new StreamReader(fileStream);

                fileStream.Position = 0;
                var content = reader.ReadToEnd();

                if (string.IsNullOrWhiteSpace(content))
                {
                    return Activator.CreateInstance<T>() ?? throw new InvalidOperationException("Failed to create default instance of type.");
                }

                var value = JsonSerializer.Deserialize<T>(content,
                    new JsonSerializerOptions() { TypeInfoResolver = new DefaultJsonTypeInfoResolver() });
                if (value is null)
                {
                    throw new InvalidDataException("Failed to deserialize data from file: content is not in valid format.");
                }

                return value;
            }
            catch (JsonException ex)
            {
                throw new InvalidDataException("File content is not valid JSON.", ex);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
