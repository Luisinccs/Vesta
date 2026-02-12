// Date: 2026-02-11
using Microsoft.Extensions.DependencyInjection;
using Vesta.Core.Interfaces;
using Vesta.Core.Services;
using Vesta.Console.Services;
using Vesta.Infrastructure.AI;
using Vesta.Infrastructure.OCR;
using Vesta.UI.ViewModels;

namespace Vesta.Console;

/// <summary>Punto de entrada de la aplicación de consola para pruebas sin MAUI.</summary>
public class Program {
    public static async Task Main(string[] args) {
        // Forzar a Mac a buscar dylibs en la carpeta del ejecutable
        if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.OSX)) {
            Environment.SetEnvironmentVariable("DYLD_LIBRARY_PATH", AppDomain.CurrentDomain.BaseDirectory);
            Environment.SetEnvironmentVariable("LD_LIBRARY_PATH", AppDomain.CurrentDomain.BaseDirectory);
        }

        System.Console.WriteLine("╔══════════════════════════════════════════╗");
        System.Console.WriteLine("║       🔒 VESTA - Modo Consola           ║");
        System.Console.WriteLine("║   Auditoría Inteligente de Contratos     ║");
        System.Console.WriteLine("╚══════════════════════════════════════════╝");
        System.Console.WriteLine();

        // Configurar DI
        var services = new ServiceCollection();
        
        // Servicios de plataforma (consola)
        services.AddSingleton<IPathService, ConsolePathService>();
        services.AddSingleton<IDocumentPicker, ConsoleDocumentPicker>();
        
        // Islas tecnológicas (compartidas desde Vesta.Core)
        services.AddSingleton<IAISocket, GemmaInferenceService>();
        services.AddSingleton<IOCRSocket, LocalOcrService>();
        services.AddSingleton<ILegalSocket, LegalAnalysisService>();
        
        // ViewModel
        services.AddTransient<MainAuditorViewModel>();

        var provider = services.BuildServiceProvider();

        // Inicializar servicios
        System.Console.Write("Inicializando servicios de IA... ");
        var aiSocket = provider.GetRequiredService<IAISocket>();
        await aiSocket.InitializeAsync();
        System.Console.WriteLine("✓");

        System.Console.Write("Inicializando motor OCR... ");
        var ocrSocket = provider.GetRequiredService<IOCRSocket>();
        await ocrSocket.InitializeAsync();
        System.Console.WriteLine("✓");
        System.Console.WriteLine();

        // Menú principal
        while (true) {
            System.Console.WriteLine("┌─────────────────────────────────┐");
            System.Console.WriteLine("│  1. Analizar documento          │");
            System.Console.WriteLine("│  2. Estado de servicios         │");
            System.Console.WriteLine("│  3. Salir                       │");
            System.Console.WriteLine("└─────────────────────────────────┘");
            System.Console.Write("Seleccione una opción: ");

            var option = System.Console.ReadLine()?.Trim();

            switch (option) {
                case "1":
                    await RunAnalysis(provider);
                    break;
                case "2":
                    ShowServiceStatus(provider);
                    break;
                case "3":
                    System.Console.WriteLine("¡Hasta luego!");
                    return;
                default:
                    System.Console.WriteLine("Opción no válida.");
                    break;
            }
            System.Console.WriteLine();
        }
    }

    private static async Task RunAnalysis(IServiceProvider provider) {
        var viewModel = provider.GetRequiredService<MainAuditorViewModel>();
        
        System.Console.WriteLine("\n--- Iniciando análisis ---");
        await viewModel.AnalyzeDocumentAsync();
        
        System.Console.WriteLine($"\nEstado: {viewModel.StatusMessage}");
        
        if (viewModel.CurrentReport != null) {
            var report = viewModel.CurrentReport;
            System.Console.WriteLine($"\n📄 Documento: {report.DocumentName}");
            System.Console.WriteLine($"📅 Fecha: {report.AnalysisDate:yyyy-MM-dd HH:mm}");
            System.Console.WriteLine($"⚠️  Riesgo: {report.RiskLevel}");
            System.Console.WriteLine($"📝 Resumen: {report.Summary}");
            
            if (report.Findings.Count > 0) {
                System.Console.WriteLine($"\n🔍 Hallazgos ({report.Findings.Count}):");
                foreach (var finding in report.Findings) {
                    var icon = finding.Severity switch {
                        Core.Models.HealthLevel.Red => "🔴",
                        Core.Models.HealthLevel.Amber => "🟡",
                        Core.Models.HealthLevel.Green => "🟢",
                        _ => "⚪"
                    };
                    System.Console.WriteLine($"  {icon} [{finding.Severity}] {finding.ClauseTitle}");
                    System.Console.WriteLine($"     {finding.RiskDescription}");
                    System.Console.WriteLine($"     → {finding.SuggestedAction}");
                }
            }
        }
    }

    private static void ShowServiceStatus(IServiceProvider provider) {
        var ai = provider.GetRequiredService<IAISocket>();
        var ocr = provider.GetRequiredService<IOCRSocket>();

        System.Console.WriteLine("\n--- Estado de Servicios ---");
        
        if (ai is GemmaInferenceService gemma) {
            System.Console.WriteLine($"  IA (Gemma 2B): {(gemma.UseMockMode ? "Mock" : "Real")}");
        }
        
        if (ocr is LocalOcrService ocrService) {
            System.Console.WriteLine($"  OCR (Tesseract): {(ocrService.UseMockMode ? "Mock" : "Real")}");
        }

        var pathService = provider.GetRequiredService<IPathService>();
        System.Console.WriteLine($"  AppData: {pathService.GetAppDataDirectory()}");
        System.Console.WriteLine($"  Modelos: {pathService.GetModelsDirectory()}");
    }
}
