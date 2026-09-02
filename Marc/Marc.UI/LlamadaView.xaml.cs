using System.Windows;
using System.Windows.Controls;
using Marc.Core;
using Marc.Data;
using Marc.Servicios;

namespace Marc.UI;

public partial class LlamadaView : UserControl
{
    private readonly Tema _tema;
    private readonly SesionRepository _sesionRepository = new();
    private GestorConversacion? _gestor;
    private bool _llamadaActiva = true;

    public LlamadaView(Tema tema)
    {
        InitializeComponent();
        MessageBox.Show($"Ensamblado Core (LlamadaView): {typeof(ConfiguracionConversacion).Assembly.Location}");
        _tema = tema;
        TxtNombreTema.Text = tema.Nombre;

        int idSesion = _sesionRepository.IniciarSesion(tema.IdTema);

        _gestor = new GestorConversacion(
            idSesion: idSesion,
            nombreUsuario: "Alex",
            nivelIngles: "B1",
            nombreTema: tema.Nombre,
            promptBaseTema: tema.PromptBase,
            transcriptor: new TranscriptorAzure(),
            tutor: new TutorGroq(),
            sintetizador: new SintetizadorAzure(),
            tutorRespaldo: new TutorGemini());
    }

    private async void BtnMicrofono_Click(object sender, RoutedEventArgs e)
    {
        BtnMicrofono.IsEnabled = false;
        await CicloDeConversacionAsync();
    }

    private async Task CicloDeConversacionAsync()
    {
        while (_llamadaActiva)
        {
            TxtEstado.Text = "Escuchando...";
            BtnMicrofono.Content = "Escuchando...";
            BurbujaUsuario.Visibility = Visibility.Collapsed;
            TxtPuntaje.Visibility = Visibility.Collapsed;

            bool primeraPalabraDelTurno = true;

            ResultadoTurno? resultado;

            try
            {
                resultado = await _gestor!.EjecutarTurnoAsync(
                    alTranscribirUsuario: texto => Dispatcher.Invoke(() =>
                    {
                        TxtMensajeUsuario.Text = texto;
                        BurbujaUsuario.Visibility = Visibility.Visible;
                        TxtEstado.Text = "Ada esta respondiendo...";
                    }),
                    alConocerPuntaje: puntaje => Dispatcher.Invoke(() =>
                    {
                        TxtPuntaje.Text = $"{puntaje}/10";
                        TxtPuntaje.Visibility = Visibility.Visible;
                    }),
                    alPronunciarPalabra: palabra => Dispatcher.Invoke(() =>
                    {
                        if (primeraPalabraDelTurno)
                        {
                            TxtMensajeAda.Text = string.Empty;
                            primeraPalabraDelTurno = false;
                        }

                        TxtMensajeAda.Text = string.IsNullOrEmpty(TxtMensajeAda.Text)
                            ? palabra
                            : TxtMensajeAda.Text + " " + palabra;
                    }));
            }
            catch (Exception ex) when (ex is InvalidOperationException or TaskCanceledException)
            {
                TxtEstado.Text = "Hubo un problema de conexion. Intenta de nuevo.";
                BtnMicrofono.Content = "Hablar";
                BtnMicrofono.IsEnabled = true;
                return;
            }

            if (!_llamadaActiva)
                return;

            if (resultado is null)
            {
                TxtEstado.Text = "No te escuche. Intenta de nuevo.";
                BtnMicrofono.Content = "Hablar";
                BtnMicrofono.IsEnabled = true;
                return;
            }
        }
    }
    private void BtnColgar_Click(object sender, RoutedEventArgs e)
    {
        _llamadaActiva = false;

        if (_gestor is not null)
            _sesionRepository.CerrarSesion(_gestor.IdSesion, _gestor.CalcularPuntajePromedio());

        var mainWindow = (MainWindow)Window.GetWindow(this)!;
        mainWindow.VolverAlMenuPrincipal();
    }

    private void BtnAjustes_Click(object sender, RoutedEventArgs e)
    {
        var ventana = new AjustesWindow { Owner = Window.GetWindow(this) };
        ventana.ShowDialog();
    }
}