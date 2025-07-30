using SmartParkingSystem.Interfaces;
using Serilog;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

namespace InventoryApp.Services
{
    internal class FileService<T> : IFileService<T>
    {
        private readonly ILogger _logger = Log.ForContext<FileService<T>>();

        private void CreateDirectory(string path)
        {
            var directoryName = Path.GetDirectoryName(path);
            if (directoryName is not null && !Directory.Exists(directoryName))
            {
                Directory.CreateDirectory(directoryName);
                _logger.Information("Created directory at path: {DirectoryPath}", directoryName);
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

                _logger.Information("Writing to file: {FilePath}", filePath);
                CreateDirectory(filePath);

                var json = JsonSerializer.Serialize(value, new JsonSerializerOptions { WriteIndented = true });
                using FileStream fileStream = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
                using StreamWriter writer = new StreamWriter(fileStream);

                fileStream.SetLength(0);
                fileStream.Position = 0;
                writer.Write(json);

                _logger.Information("Successfully wrote data to file: {FilePath}", filePath);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "An error occurred while writing to file: {FilePath}", filePath);
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

                _logger.Information("Reading from file: {FilePath}", filePath);
                CreateDirectory(filePath);

                using FileStream fileStream = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
                using StreamReader reader = new StreamReader(fileStream);

                fileStream.Position = 0;
                var content = reader.ReadToEnd();

                if (string.IsNullOrWhiteSpace(content))
                {
                    _logger.Warning("File {FilePath} is empty. Returning new instance of {Type}.", filePath, typeof(T).Name);
                    return Activator.CreateInstance<T>() ?? throw new InvalidOperationException("Failed to create default instance of type.");
                }

                var value = JsonSerializer.Deserialize<T>(content,
                    new JsonSerializerOptions() { TypeInfoResolver = new DefaultJsonTypeInfoResolver() });

                if (value is null)
                {
                    _logger.Error("Deserialization failed: content of {FilePath} is invalid.", filePath);
                    throw new InvalidDataException("Failed to deserialize data from file: content is not in valid format.");
                }

                _logger.Information("Successfully read and deserialized file: {FilePath}", filePath);
                return value;
            }
            catch (JsonException ex)
            {
                _logger.Error(ex, "Invalid JSON format in file: {FilePath}", filePath);
                throw new InvalidDataException("File content is not valid JSON.", ex);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "An error occurred while reading from file: {FilePath}", filePath);
                throw;
            }
        }
    }
}
