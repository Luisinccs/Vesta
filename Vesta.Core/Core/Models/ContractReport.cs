// Date: 2026-02-10
namespace Vesta.Core.Models;

/// <summary>Nivel de riesgo o salud de un hallazgo o contrato.</summary>
public enum HealthLevel {
    Green,
    Amber,
    Red
}

/// <summary>Representa un hallazgo específico dentro de una cláusula contractual.</summary>
public record Finding {
    /// <summary>Título de la cláusula analizada.</summary>
    public string ClauseTitle { get; init; } = string.Empty;
    /// <summary>Descripción del riesgo detectado.</summary>
    public string RiskDescription { get; init; } = string.Empty;
    /// <summary>Severidad del riesgo detectado.</summary>
    public HealthLevel Severity { get; init; }
    /// <summary>Acción sugerida para mitigar el riesgo.</summary>
    public string SuggestedAction { get; init; } = string.Empty;
}

/// <summary>Modelo de datos inmutable para el reporte final de auditoría de contratos.</summary>
public record ContractReport {
    /// <summary>Nombre del archivo analizado.</summary>
    public string DocumentName { get; init; } = string.Empty;
    /// <summary>Fecha en la que se realizó la auditoría.</summary>
    public DateTime AnalysisDate { get; init; }
    /// <summary>Nivel de riesgo general del contrato.</summary>
    public HealthLevel RiskLevel { get; init; }
    /// <summary>Resumen ejecutivo de los hallazgos.</summary>
    public string Summary { get; init; } = string.Empty;
    /// <summary>Lista detallada de hallazgos específicos.</summary>
    public List<Finding> Findings { get; init; } = new();
}
