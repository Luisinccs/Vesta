// Date: 2026-02-10
namespace Vesta.Core.Interfaces;

/// <summary>Interfaz para la extracción local de texto desde imágenes (OCR).</summary>
public interface IOCRSocket : ISocket {
    /// <summary>Extrae texto de un flujo de imagen de forma asíncrona.</summary>
    Task<string> ExtractTextAsync(Stream imageStream);
}
