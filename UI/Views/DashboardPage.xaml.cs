// Date: 2026-02-10
using Vesta.UI.ViewModels;

namespace Vesta.UI.Views;

/// <summary>Página principal de visualización de resultados de auditoría y activación de análisis.</summary>
public partial class DashboardPage : ContentPage {
	/// <summary>Inicializa la página y vincula manualmente el ViewModel para pruebas de UI.</summary>
	public DashboardPage(MainAuditorViewModel viewModel) {
		InitializeComponent();
		BindingContext = viewModel;
	}

	/// <summary>Sobrecarga obligatoria para el compilador de XAML en modo diseño.</summary>
	public DashboardPage() {
		InitializeComponent();
	}
}
