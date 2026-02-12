// Date: 2026-02-11
using Vesta.Core.Interfaces;

namespace Vesta.Services;

/// <summary>Implementación de IPathService que utiliza las APIs de MAUI (FileSystem) para la resolución de rutas.</summary>
public class MauiPathService : IPathService {
    /// <summary>Obtiene el directorio de datos de la aplicación usando la API de MAUI.</summary>
    public string GetAppDataDirectory() => FileSystem.AppDataDirectory;

    /// <summary>Obtiene la ruta del directorio de modelos de IA.</summary>
    public string GetModelsDirectory() {
        var dir = Path.Combine(FileSystem.AppDataDirectory, "Models");
        if (!Directory.Exists(dir)) {
            Directory.CreateDirectory(dir);
        }
        return dir;
    }

    /// <summary>Obtiene la ruta para los datos de entrenamiento de Tesseract OCR.</summary>
    public string GetTesseractDataDirectory() {
        var dir = Path.Combine(FileSystem.AppDataDirectory, "TessData");
        if (!Directory.Exists(dir)) {
            Directory.CreateDirectory(dir);
        }
        return dir;
    }

    /// <summary>Obtiene la ruta local de un archivo de modelo, aprovisionándolo desde el paquete si es necesario.</summary>
    public async Task<string> GetModelPathAsync(string fileName) {
#if DEBUG && WINDOWS
        var devPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName.EndsWith(".traineddata") ? "TessData" : "Models", fileName);
        if (File.Exists(devPath)) {
            return devPath;
        }
#endif

        var targetDir = Path.Combine(FileSystem.AppDataDirectory, fileName.EndsWith(".traineddata") ? "TessData" : "Models");
        var targetPath = Path.Combine(targetDir, fileName);

        if (File.Exists(targetPath)) {
            return targetPath;
        }

        if (!Directory.Exists(targetDir)) {
            Directory.CreateDirectory(targetDir);
        }

        try {
            string sourcePath = fileName.EndsWith(".traineddata") ? $"TessData/{fileName}" : $"Models/{fileName}";
            using var sourceStream = await FileSystem.OpenAppPackageFileAsync(sourcePath);
            using var destinationStream = File.Create(targetPath);
            await sourceStream.CopyToAsync(destinationStream);
        } catch (Exception ex) {
            Console.WriteLine($"[MauiPathService Error] No se pudo copiar {fileName}: {ex.Message}");
            throw;
        }

        return targetPath;
    }

    /// <summary>Abre un archivo empaquetado con la aplicación usando la API de MAUI.</summary>
    public async Task<Stream> OpenAppPackageFileAsync(string filePath) {
        return await FileSystem.OpenAppPackageFileAsync(filePath);
    }
}
