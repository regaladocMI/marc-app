using System.Windows;
using Marc.Core;

namespace Marc.UI;

public partial class AjustesWindow : Window
{
    public AjustesWindow()
    {
        InitializeComponent();
        SliderVelocidad.Value = ConfiguracionConversacion.VelocidadHabla;
        SliderPaciencia.Value = ConfiguracionConversacion.PacienciaSegundos;
    }

    private void SliderVelocidad_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        ConfiguracionConversacion.VelocidadHabla = e.NewValue;
        TxtVelocidad.Text = $"{e.NewValue:F1}x";
    }

    private void SliderPaciencia_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        ConfiguracionConversacion.PacienciaSegundos = (int)e.NewValue;
        TxtPaciencia.Text = $"{(int)e.NewValue}s";
    }

    private void BtnCerrar_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }
}