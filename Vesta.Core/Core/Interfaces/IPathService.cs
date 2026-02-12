// Date: 2026-02-11
namespace Vesta.Core.Interfaces;

/// <summary>Abstracción para el acceso a rutas de archivos y aprovisionamiento de modelos.</summary>
public interface IPathService {
    /// <summary>Obtiene el directorio base de datos de la aplicación.</summary>
    string GetAppDataDirectory();
    /// <summary>Obtiene la ruta del directorio de modelos de IA.</summary>
    string GetModelsDirectory();
    /// <summary>Obtiene la ruta del directorio de datos de Tesseract OCR.</summary>
    string GetTesseractDataDirectory();
    /// <summary>Obtiene la ruta local de un archivo de modelo, aprovisionándolo si es necesario.</summary>
    Task<string> GetModelPathAsync(string fileName);
    /// <summary>Abre un archivo empaquetado con la aplicación como un Stream.</summary>
    Task<Stream> OpenAppPackageFileAsync(string filePath);
}
