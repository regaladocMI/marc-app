using System.Windows.Controls;
using Marc.Data;

namespace Marc.UI;

public partial class VocabularioView : UserControl
{
    private readonly ReporteRepository _reporteRepository = new();

    public VocabularioView()
    {
        InitializeComponent();
        ListaVocabulario.ItemsSource = _reporteRepository.ObtenerVocabularioCompleto().DefaultView;
    }
}