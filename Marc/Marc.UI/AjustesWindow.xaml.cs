using System.Windows;
using Marc.Core;
using Marc.Data;

namespace Marc.UI;

public partial class AjustesWindow : Window
{
    private readonly ConfiguracionUsuarioRepository _configuracionRepository = new();
    private bool _cargando = true;

    public AjustesWindow()
    {
        InitializeComponent();

        SliderVelocidad.Value = ConfiguracionConversacion.VelocidadHabla;
        TxtVelocidad.Text = $"{ConfiguracionConversacion.VelocidadHabla:F1}x";

        SliderPaciencia.Value = ConfiguracionConversacion.PacienciaSegundos;
        TxtPaciencia.Text = $"{ConfiguracionConversacion.PacienciaSegundos}s";

        _cargando = false;
    }

    private void SliderVelocidad_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        if (_cargando)
            return;

        ConfiguracionConversacion.VelocidadHabla = e.NewValue;
        TxtVelocidad.Text = $"{e.NewValue:F1}x";
        GuardarCambios();
    }

    private void SliderPaciencia_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        if (_cargando)
            return;

        ConfiguracionConversacion.PacienciaSegundos = (int)e.NewValue;
        TxtPaciencia.Text = $"{(int)e.NewValue}s";
        GuardarCambios();
    }

    private void GuardarCambios()
    {
        _configuracionRepository.Actualizar(
            (decimal)ConfiguracionConversacion.VelocidadHabla,
            ConfiguracionConversacion.PacienciaSegundos);
    }

    private void BtnCerrar_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }
}