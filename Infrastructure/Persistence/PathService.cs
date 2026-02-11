// Date: 2026-02-10
namespace Vesta.Infrastructure.Persistence;

/// <summary>Servicio encargado de la gestión de rutas y provisionamiento de archivos de IA y modelos locales.</summary>
public class PathService {
    /// <summary>Obtiene la ruta local de un archivo de modelo, optimizando el acceso en desarrollo Windows.</summary>
    public async Task<string> GetModelPathAsync(string fileName) {
#if DEBUG && WINDOWS
        // En desarrollo Windows, apuntamos directamente a la carpeta de salida para evitar duplicar 3GB en AppData
        var devPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName.EndsWith(".traineddata") ? "TessData" : "Models", fileName);
        if (File.Exists(devPath)) {
            return devPath;
        }
#endif

        // Carpeta de destino en el almacenamiento local de la app (Producción o Mobile)
        var targetDir = Path.Combine(FileSystem.AppDataDirectory, fileName.EndsWith(".traineddata") ? "TessData" : "Models");
        var targetPath = Path.Combine(targetDir, fileName);

        if (File.Exists(targetPath)) {
            return targetPath;
        }

        // Asegurar que el directorio existe
        if (!Directory.Exists(targetDir)) {
            Directory.CreateDirectory(targetDir);
        }

        // Copiar archivo desde el paquete de la aplicación (Resources/Raw)
        try {
            string sourcePath = fileName.EndsWith(".traineddata") ? $"TessData/{fileName}" : $"Models/{fileName}";
            
            using var sourceStream = await FileSystem.OpenAppPackageFileAsync(sourcePath);
            using var destinationStream = File.Create(targetPath);
            await sourceStream.CopyToAsync(destinationStream);
        } catch (Exception ex) {
            Console.WriteLine($"[PathService Error] No se pudo copiar {fileName}: {ex.Message}");
            throw;
        }

        return targetPath;
    }

    /// <summary>Obtiene la ruta de la carpeta donde se almacenan los modelos de IA.</summary>
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
}
