// Date: 2026-02-11
using Vesta.Core.Interfaces;

namespace Vesta.Console.Services;

/// <summary>Implementación de IPathService para aplicaciones de consola. Usa rutas locales relativas al ejecutable.</summary>
public class ConsolePathService : IPathService {
    private readonly string _appDataDirectory;

    public ConsolePathService() {
        _appDataDirectory = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "Vesta");

        if (!Directory.Exists(_appDataDirectory)) {
            Directory.CreateDirectory(_appDataDirectory);
        }
    }

    public string GetAppDataDirectory() => _appDataDirectory;

    public string GetModelsDirectory() {
        // En consola, preferimos la carpeta local del binario si existe
        var localDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Models");
        if (Directory.Exists(localDir) && File.Exists(Path.Combine(localDir, "model.onnx"))) {
            return localDir;
        }

        var dir = Path.Combine(_appDataDirectory, "Models");
        if (!Directory.Exists(dir)) {
            Directory.CreateDirectory(dir);
        }
        return dir;
    }

    public string GetTesseractDataDirectory() {
        // En consola, preferimos la carpeta local del binario si existe
        var localDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TessData");
        if (Directory.Exists(localDir) && Directory.GetFiles(localDir, "*.traineddata").Length > 0) {
            return localDir;
        }

        var dir = Path.Combine(_appDataDirectory, "TessData");
        if (!Directory.Exists(dir)) {
            Directory.CreateDirectory(dir);
        }
        return dir;
    }

    public async Task<string> GetModelPathAsync(string fileName) {
        var localPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, 
            fileName.EndsWith(".traineddata") ? "TessData" : "Models", fileName);
        
        if (File.Exists(localPath)) {
            return await Task.FromResult(localPath);
        }

        var targetDir = Path.Combine(_appDataDirectory, fileName.EndsWith(".traineddata") ? "TessData" : "Models");
        var targetPath = Path.Combine(targetDir, fileName);

        if (File.Exists(targetPath)) {
            return targetPath;
        }

        System.Console.WriteLine($"[ConsolePathService] Archivo no encontrado: {fileName}");
        return await Task.FromResult(targetPath);
    }

    public Task<Stream> OpenAppPackageFileAsync(string filePath) {
        var localPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, filePath);
        if (File.Exists(localPath)) {
            return Task.FromResult<Stream>(File.OpenRead(localPath));
        }
        throw new FileNotFoundException($"Archivo no encontrado: {localPath}");
    }
}
