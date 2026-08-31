using System.Windows;
using System.Windows.Controls;
using Marc.Data;

namespace Marc.UI;

public partial class ReportesView : UserControl
{
    private readonly ReporteRepository _reporteRepository = new();

    public ReportesView()
    {
        InitializeComponent();

        FechaDesde.SelectedDate = DateTime.Today.AddMonths(-1);
        FechaHasta.SelectedDate = DateTime.Today;

        CalcularPromedioPorTema();
        CalcularVocabulario();
    }

    private void BtnCalcularPromedio_Click(object sender, RoutedEventArgs e) => CalcularPromedioPorTema();

    private void BtnCalcularVocabulario_Click(object sender, RoutedEventArgs e) => CalcularVocabulario();

    private void CalcularPromedioPorTema()
    {
        if (FechaDesde.SelectedDate is null || FechaHasta.SelectedDate is null)
        {
            MessageBox.Show("Selecciona ambas fechas.");
            return;
        }

        GrillaPromedioPorTema.ItemsSource = _reporteRepository
            .ObtenerPuntajePromedioPorTema(FechaDesde.SelectedDate.Value, FechaHasta.SelectedDate.Value)
            .DefaultView;
    }

    private void CalcularVocabulario()
    {
        if (!int.TryParse(TxtTopN.Text, out int topN) || topN <= 0)
        {
            MessageBox.Show("Ingresa un numero valido mayor a cero.");
            return;
        }

        GrillaVocabulario.ItemsSource = _reporteRepository
            .ObtenerVocabularioMasRepetido(topN)
            .DefaultView;
    }
}