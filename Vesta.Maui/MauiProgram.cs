// Date: 2026-02-11
using Microsoft.Extensions.Logging;
using Vesta.Core.Interfaces;
using Vesta.Core.Services;
using Vesta.Infrastructure.AI;
using Vesta.Infrastructure.OCR;
using Vesta.Infrastructure.Persistence;
using Vesta.Services;
using Vesta.UI.ViewModels;
using Vesta.UI.Views;

namespace Vesta;

/// <summary>Configuracion central del ciclo de vida de la aplicacion MAUI.</summary>
public static class MauiProgram {
    /// <summary>Crea y configura la aplicacion con sus dependencias e islas tecnologicas.</summary>
    public static MauiApp CreateMauiApp() {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts => {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

#if DEBUG
        builder.Logging.AddDebug();
#endif

        // Registro de Servicios de Plataforma (MAUI-specificos)
        builder.Services.AddSingleton<IPathService, MauiPathService>();
        builder.Services.AddSingleton<IDocumentPicker, MauiDocumentPicker>();

        // Registro de Islas Tecnologicas (Infrastructure) como Singletons
        builder.Services.AddSingleton<IAISocket, GemmaInferenceService>();
        builder.Services.AddSingleton<IOCRSocket, LocalOcrService>();
        
        // Registro de Cerebro Legal (Core Services)
        builder.Services.AddSingleton<ILegalSocket, LegalAnalysisService>();

        // Registro de UI (ViewModels y Views) como Transients
        builder.Services.AddTransient<MainAuditorViewModel>();
        builder.Services.AddTransient<DashboardPage>();
        builder.Services.AddSingleton<AppShell>();
        builder.Services.AddSingleton<App>();

        return builder.Build();
    }
}
