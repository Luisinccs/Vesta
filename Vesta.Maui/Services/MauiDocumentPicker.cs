// Date: 2026-02-11
using Vesta.Core.Interfaces;

namespace Vesta.Services;

/// <summary>Implementación de IDocumentPicker que utiliza la API de MAUI (FilePicker).</summary>
public class MauiDocumentPicker : IDocumentPicker {
    /// <summary>Permite al usuario seleccionar un documento utilizando el picker nativo de la plataforma.</summary>
    public async Task<(Stream? stream, string? fileName)> PickDocumentAsync() {
        var result = await FilePicker.Default.PickAsync(new PickOptions {
            PickerTitle = "Seleccione un contrato (PDF o Imagen)",
            FileTypes = FilePickerFileType.Images
        });

        if (result == null) {
            return (null, null);
        }

        var stream = await result.OpenReadAsync();
        return (stream, result.FileName);
    }
}
