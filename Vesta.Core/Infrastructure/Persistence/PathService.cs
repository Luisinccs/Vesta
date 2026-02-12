// Date: 2026-02-11
using Vesta.Core.Interfaces;

namespace Vesta.Infrastructure.Persistence;

/// <summary>Servicio base de gestión de rutas que utiliza la abstracción IPathService.</summary>
public class PathService : IPathService {
    private readonly string _appDataDirectory;
    private readonly Func<string, Task<Stream>>? _packageFileOpener;

    /// <summary>Constructor para inyección — recibe la ruta base y un opener de archivos empaquetados.</summary>
    public PathService(string appDataDirectory, Func<string, Task<Stream>>? packageFileOpener = null) {
        _appDataDirectory = appDataDirectory;
        _packageFileOpener = packageFileOpener;
    }

    /// <summary>Obtiene el directorio base de datos de la aplicación.</summary>
    public string GetAppDataDirectory() => _appDataDirectory;

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
        var targetDir = Path.Combine(_appDataDirectory, fileName.EndsWith(".traineddata") ? "TessData" : "Models");
        var targetPath = Path.Combine(targetDir, fileName);

        if (File.Exists(targetPath)) {
            return targetPath;
        }

        // Asegurar que el directorio existe
        if (!Directory.Exists(targetDir)) {
            Directory.CreateDirectory(targetDir);
        }

        // Copiar archivo desde el paquete de la aplicación (Resources/Raw)
        if (_packageFileOpener != null) {
            try {
                string sourcePath = fileName.EndsWith(".traineddata") ? $"TessData/{fileName}" : $"Models/{fileName}";

                using var sourceStream = await _packageFileOpener(sourcePath);
                using var destinationStream = File.Create(targetPath);
                await sourceStream.CopyToAsync(destinationStream);
            } catch (Exception ex) {
                Console.WriteLine($"[PathService Error] No se pudo copiar {fileName}: {ex.Message}");
                throw;
            }
        } else {
            Console.WriteLine($"[PathService Warning] No hay opener de paquetes, archivo {fileName} no disponible.");
        }

        return targetPath;
    }

    /// <summary>Obtiene la ruta de la carpeta donde se almacenan los modelos de IA.</summary>
    public string GetModelsDirectory() {
        var dir = Path.Combine(_appDataDirectory, "Models");
        if (!Directory.Exists(dir)) {
            Directory.CreateDirectory(dir);
        }
        return dir;
    }

    /// <summary>Obtiene la ruta para los datos de entrenamiento de Tesseract OCR.</summary>
    public string GetTesseractDataDirectory() {
        var dir = Path.Combine(_appDataDirectory, "TessData");
        if (!Directory.Exists(dir)) {
            Directory.CreateDirectory(dir);
        }
        return dir;
    }

    /// <summary>Abre un archivo empaquetado con la aplicación como un Stream.</summary>
    public async Task<Stream> OpenAppPackageFileAsync(string filePath) {
        if (_packageFileOpener != null) {
            return await _packageFileOpener(filePath);
        }
        // Fallback: intentar abrir desde el directorio del ejecutable
        var localPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, filePath);
        return File.OpenRead(localPath);
    }
}
