// Date: 2026-02-10
namespace Vesta.Core.Interfaces;

/// <summary>Interfaz para la inferencia semántica local utilizando modelos de IA.</summary>
public interface IAISocket : ISocket {
    /// <summary>Analiza un texto para extraer hallazgos semánticos de forma asíncrona.</summary>
    Task<string> AnalyzeTextAsync(string text);
}
