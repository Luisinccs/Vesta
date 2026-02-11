// Date: 2026-02-10
using Vesta.Core.Models;

namespace Vesta.Core.Interfaces;

/// <summary>Interfaz para la lógica de clasificación de riesgos y procesamiento legal.</summary>
public interface ILegalSocket : ISocket {
    /// <summary>Procesa los hallazgos crudos para generar un reporte detallado del contrato.</summary>
    Task<ContractReport> ProcessFindingsAsync(string rawFindings);
}
