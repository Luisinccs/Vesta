// Date: 2026-02-10
namespace Vesta.Core.Interfaces;

/// <summary>Interfaz base para el ciclo de vida de los componentes del sistema.</summary>
public interface ISocket {
    /// <summary>Inicializa el componente de forma asíncrona.</summary>
    Task InitializeAsync();
}
