using System.Windows;
using Marc.Data;

namespace Marc.UI;

public partial class DetalleSesionWindow : Window
{
    public DetalleSesionWindow(int idSesion)
    {
        InitializeComponent();

        var reporteRepository = new ReporteRepository();
        GrillaMensajes.ItemsSource = reporteRepository.ObtenerMensajesDeSesion(idSesion).DefaultView;
    }
}