// Date: 2026-02-11
using Vesta.Core.Interfaces;

namespace Vesta.Console.Services;

/// <summary>Implementación de IDocumentPicker para consola. Pide una ruta de archivo al usuario por stdin.</summary>
public class ConsoleDocumentPicker : IDocumentPicker {
    public Task<(Stream? stream, string? fileName)> PickDocumentAsync() {
        System.Console.Write("Ingrese la ruta del documento a analizar (o 'cancelar'): ");
        var path = System.Console.ReadLine()?.Trim();

        if (string.IsNullOrWhiteSpace(path) || path.Equals("cancelar", StringComparison.OrdinalIgnoreCase)) {
            return Task.FromResult<(Stream?, string?)>((null, null));
        }

        if (!File.Exists(path)) {
            System.Console.WriteLine($"Error: El archivo '{path}' no existe.");
            return Task.FromResult<(Stream?, string?)>((null, null));
        }

        var stream = (Stream)File.OpenRead(path);
        var fileName = Path.GetFileName(path);
        return Task.FromResult<(Stream?, string?)>((stream, fileName));
    }
}
