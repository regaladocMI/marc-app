using System.Windows;
using System.Windows.Controls;
using Marc.Data;
using System.Data;

namespace Marc.UI;

public partial class HistorialView : UserControl
{
    private readonly ReporteRepository _reporteRepository = new();

    public HistorialView()
    {
        InitializeComponent();

        FechaDesde.SelectedDate = DateTime.Today.AddDays(-30);
        FechaHasta.SelectedDate = DateTime.Today;

        BuscarSesiones();
    }

    private void GrillaSesiones_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        if (GrillaSesiones.SelectedItem is not DataRowView filaSeleccionada)
            return;

        int idSesion = (int)filaSeleccionada["id_sesion"];

        var ventana = new DetalleSesionWindow(idSesion) { Owner = Window.GetWindow(this) };
        ventana.ShowDialog();
    }

    private void BtnBuscar_Click(object sender, RoutedEventArgs e)
    {
        BuscarSesiones();
    }

    private void BuscarSesiones()
    {
        if (FechaDesde.SelectedDate is null || FechaHasta.SelectedDate is null)
        {
            MessageBox.Show("Selecciona ambas fechas.");
            return;
        }

        GrillaSesiones.ItemsSource = _reporteRepository
            .ObtenerProgresoPorFecha(FechaDesde.SelectedDate.Value, FechaHasta.SelectedDate.Value)
            .DefaultView;
    }
}