// Date: 2026-02-10
using Vesta.Core.Interfaces;
using Vesta.Infrastructure.Persistence;
using Microsoft.ML.OnnxRuntimeGenAI;

namespace Vesta.Infrastructure.AI;

/// <summary>Servicio de inferencia real que utiliza Gemma 2B y ONNX Runtime GenAI.</summary>
public class GemmaInferenceService : IAISocket, IDisposable {
    private readonly PathService _pathService;
    private Model? _model;
    private Tokenizer? _tokenizer;
    private bool _useMockMode = true;

    /// <summary>Obtiene o establece si el servicio debe operar en modo simulado.</summary>
    public bool UseMockMode {
        get => _useMockMode;
        set => _useMockMode = value;
    }

    /// <summary>Inicializa el servicio inyectando el gestor de rutas.</summary>
    public GemmaInferenceService(PathService pathService) {
        _pathService = pathService;
    }

    /// <summary>Inicializa el motor de Gemma cargando el modelo y el tokenizador desde el almacenamiento local.</summary>
    public async Task InitializeAsync() {
        if (_useMockMode && !await CheckModelFilesAsync()) {
            Console.WriteLine("Gemma 2B: Archivos no encontrados, permaneciendo en modo Mock.");
            return;
        }

        try {
            var modelPath = _pathService.GetModelsDirectory();
            
            // Aseguramos el aprovisionamiento de archivos clave
            await _pathService.GetModelPathAsync("model.onnx");
            await _pathService.GetModelPathAsync("genai_config.json");
            await _pathService.GetModelPathAsync("tokenizer.model");

            // Carga asíncrona simulada para no bloquear la UI durante la instanciación nativa
            await Task.Run(() => {
                _model = new Model(modelPath);
                _tokenizer = new Tokenizer(_model);
            });

            _useMockMode = false;
            Console.WriteLine("Gemma 2B Engine Ready (Modo Real)");
        } catch (Exception ex) {
            Console.WriteLine($"[GemmaInferenceService Error] Error al cargar ONNX GenAI: {ex.Message}");
            _useMockMode = true; // Fallback a Mock si falla la carga real
        }
    }

    /// <summary>Genera un análisis legal utilizando el modelo Gemma 2B.</summary>
    public async Task<string> AnalyzeTextAsync(string text) {
        if (_useMockMode || _model == null || _tokenizer == null) {
            await Task.Delay(2000);
            return "MODO MOCK: Se detectó una cláusula de riesgo en el párrafo 3 referente a la rescisión.";
        }

        return await Task.Run(() => {
            try {
                var systemPrompt = "Eres un experto legal. Analiza el siguiente contrato y detecta riesgos, se conciso y directo: ";
                var fullPrompt = $"<start_of_turn>user\n{systemPrompt}\n{text}<end_of_turn>\n<start_of_turn>model\n";

                using var tokens = _tokenizer.Encode(fullPrompt);
                using var generatorParams = new GeneratorParams(_model);
                generatorParams.SetSearchOption("max_length", 512);

                using var generator = new Generator(_model, generatorParams);
                generator.AppendTokenSequences(tokens);
                string response = "";

                while (!generator.IsDone()) {
                    generator.GenerateNextToken();
                    var sequence = generator.GetSequence(0);
                    response += _tokenizer.Decode(sequence.Slice(sequence.Length - 1, 1));
                }

                return response;
            } catch (Exception ex) {
                return $"Error en inferencia: {ex.Message}";
            }
        });
    }

    private async Task<bool> CheckModelFilesAsync() {
        var dir = _pathService.GetModelsDirectory();
        return await Task.FromResult(File.Exists(Path.Combine(dir, "model.onnx")));
    }

    /// <summary>Libera los recursos nativos del modelo y tokenizador.</summary>
    public void Dispose() {
        _tokenizer?.Dispose();
        _model?.Dispose();
    }
}
