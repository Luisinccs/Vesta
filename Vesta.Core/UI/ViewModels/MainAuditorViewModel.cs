// Date: 2026-02-11
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.IO;
using Vesta.Core.Interfaces;
using Vesta.Core.Models;

namespace Vesta.UI.ViewModels;

/// <summary>ViewModel principal encargado de coordinar el flujo de auditoría entre las diferentes islas tecnológicas.</summary>
public partial class MainAuditorViewModel : ObservableObject {
    private readonly IAISocket _aiSocket;
    private readonly IOCRSocket _ocrSocket;
    private readonly ILegalSocket _legalSocket;
    private readonly IDocumentPicker _documentPicker;

    [ObservableProperty]
    private bool _isBusy;

    [ObservableProperty]
    private string _statusMessage = "Listo para auditar";

    [ObservableProperty]
    private ContractReport? _currentReport;

    /// <summary>Inicializa una nueva instancia del ViewModel con sus dependencias inyectadas.</summary>
    public MainAuditorViewModel(IAISocket aiSocket, IOCRSocket ocrSocket, ILegalSocket legalSocket, IDocumentPicker documentPicker) {
        _aiSocket = aiSocket;
        _ocrSocket = ocrSocket;
        _legalSocket = legalSocket;
        _documentPicker = documentPicker;
    }

    /// <summary>Comando que ejecuta el flujo completo de análisis de un documento.</summary>
    [RelayCommand]
    public async Task AnalyzeDocumentAsync() {
        if (IsBusy) return;

        try {
            IsBusy = true;
            Stream? documentStream = null;

            if (_aiSocket is Infrastructure.AI.GemmaInferenceService gemma && gemma.UseMockMode) {
                // En modo Mock, creamos un stream vacío para cumplir con el contrato
                documentStream = new MemoryStream();
            } else {
                StatusMessage = "Seleccionando archivo...";
                var (stream, fileName) = await _documentPicker.PickDocumentAsync();

                if (stream == null) {
                    StatusMessage = "Operación cancelada";
                    return;
                }
                documentStream = stream;
            }

            StatusMessage = "Escaneando documento...";
            
            // 1. Extracción de texto (OCR Island)
            var rawText = await _ocrSocket.ExtractTextAsync(documentStream);
            
            StatusMessage = "Analizando cláusulas con Gemma 2B...";
            // 2. Inferencia semántica (AI Island)
            var analysisJson = await _aiSocket.AnalyzeTextAsync(rawText);
            
            StatusMessage = "Generando reporte legal personalizado...";
            // 3. Procesamiento de riesgos (Legal Brain)
            CurrentReport = await _legalSocket.ProcessFindingsAsync(analysisJson);
            
            StatusMessage = "Auditoría completada con éxito";
        } catch (Exception ex) {
            StatusMessage = $"Error: {ex.Message}";
            Console.WriteLine($"[MainAuditorViewModel Error] {ex}");
        } finally {
            IsBusy = false;
        }
    }
}
