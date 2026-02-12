// Date: 2026-02-11
namespace Vesta.Core.Interfaces;

/// <summary>Abstracción para la selección de documentos desde el sistema de archivos o la galería.</summary>
public interface IDocumentPicker {
    /// <summary>Permite al usuario seleccionar un documento y retorna su contenido como Stream.</summary>
    /// <returns>Una tupla con el stream del documento y su nombre, o null si se cancela.</returns>
    Task<(Stream? stream, string? fileName)> PickDocumentAsync();
}
