// Date: 2026-02-10
using Vesta.Core.Interfaces;
using Vesta.Infrastructure.Persistence;
using Tesseract;

namespace Vesta.Infrastructure.OCR;

/// <summary>Servicio de OCR local que utiliza el motor Tesseract para la extracción de texto en español e inglés.</summary>
public class LocalOcrService : IOCRSocket {
    private readonly PathService _pathService;
    private bool _useMockMode = false;

    /// <summary>Obtiene o establece si el servicio debe operar en modo simulado.</summary>
    public bool UseMockMode {
        get => _useMockMode;
        set => _useMockMode = value;
    }

    /// <summary>Inicializa el servicio inyectando el gestor de rutas.</summary>
    public LocalOcrService(PathService pathService) {
        _pathService = pathService;
    }

    /// <summary>Prepara los diccionarios y el motor de visión.</summary>
    public async Task InitializeAsync() {
        if (_useMockMode) {
            await Task.Delay(1000);
            return;
        }

        // Aseguramos que la carpeta TessData existe (el PathService se encarga de la lógica de carpetas)
        _pathService.GetTesseractDataDirectory();
        await Task.CompletedTask;
    }

    /// <summary>Extrae texto desde un Stream de imagen utilizando Tesseract OCR.</summary>
    public async Task<string> ExtractTextAsync(Stream imageStream) {
        if (_useMockMode) {
            await Task.Delay(1500);
            return "MODO MOCK: Contrato de Arrendamiento Estándar...";
        }

        try {
            string tessDataPath;

#if WINDOWS && DEBUG
            // En Windows Debug, Tesseract requiere la carpeta 'tessdata' en el directorio de ejecución
            tessDataPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "tessdata");
            
            // Verificación preventiva para diagnóstico
            if (!Directory.Exists(tessDataPath)) {
                Console.WriteLine($"[LocalOcrService] Error: No existe el directorio {tessDataPath}");
            }
#else
            tessDataPath = _pathService.GetTesseractDataDirectory();
#endif
            
            // Tesseract requiere que los archivos .traineddata estén en la raíz de la ruta proporcionada
            using var engine = new TesseractEngine(tessDataPath, "spa+eng", EngineMode.Default);
            
            using var memoryStream = new MemoryStream();
            await imageStream.CopyToAsync(memoryStream);
            byte[] imageBytes = memoryStream.ToArray();

            using var img = Pix.LoadFromMemory(imageBytes);
            using var page = engine.Process(img);
            
            var text = page.GetText();

            if (string.IsNullOrWhiteSpace(text)) {
                throw new Exception("No se pudo detectar texto legible en la imagen.");
            }

            return text;
        } catch (TesseractException tex) {
            Console.WriteLine($"ERROR TESSERACT: {tex.Message}");
            throw new Exception("Error interno del motor OCR (Tesseract). Verifique los archivos de idioma.", tex);
        } catch (Exception ex) {
            Console.WriteLine($"[LocalOcrService Error] Falla en reconocimiento: {ex.Message}");
            throw new Exception("Error al procesar el OCR: Asegúrese de que los archivos 'spa.traineddata' y 'eng.traineddata' estén en la carpeta TessData.", ex);
        }
    }
}
