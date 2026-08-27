using Marc.Core;
using Marc.Data;
using Marc.Servicios;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Marc.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void BtnTemas_Click(object sender, RoutedEventArgs e)
        {
            ContenedorPrincipal.Content = new TemasView();
        }


        private GestorConversacion? _gestorActual;
        private SesionRepository _sesionRepository = new();

        private async void BtnProbarMicrofono_Click(object sender, RoutedEventArgs e)
        {
            if (_gestorActual is null)
            {
                int idSesion = _sesionRepository.IniciarSesion(idTema: 1); // temporal, hasta el Bloque 9

                _gestorActual = new GestorConversacion(
                    idSesion: idSesion,
                    nombreUsuario: "Alex",
                    nivelIngles: "B1",
                    nombreTema: "Conversacion libre",
                    promptBaseTema: "Charla informal de practica.",
                    transcriptor: new TranscriptorAzure(),
                    tutor: new TutorGemini(),
                    sintetizador: new SintetizadorAzure(),
                    tutorRespaldo: new TutorGroq());

                BtnCortar.IsEnabled = true;
                BtnProbarMicrofono.Content = "Hablar";
            }

            BtnProbarMicrofono.IsEnabled = false;
            BtnProbarMicrofono.Content = "Ada esta escuchando...";

            try
            {
                ResultadoTurno? resultado = await _gestorActual.EjecutarTurnoAsync();

                if (resultado is null)
                {
                    MessageBox.Show("No se detecto ninguna frase.");
                }
                else
                {
                    var r = resultado.RespuestaAda;
                    MessageBox.Show(
                        $"[Respondio: {resultado.ProveedorUsado}]\n\n" +
                        $"Vos dijiste: {resultado.TextoUsuario}\n\n" +
                        $"Ada ({r.Puntaje}/10): {r.RespuestaTexto}\n\n" +
                        (r.Correccion.HuboError
                            ? $"Correccion: \"{r.Correccion.TextoOriginal}\" -> \"{r.Correccion.TextoCorregido}\"\n{r.Correccion.Explicacion}"
                            : "Sin errores."));
                }
            }
            catch (Exception ex) when (ex is InvalidOperationException or TaskCanceledException)
            {
                MessageBox.Show($"ERROR DETALLE: {ex.GetType().Name}: {ex.Message}");
            }
            finally
            {
                BtnProbarMicrofono.IsEnabled = true;
                BtnProbarMicrofono.Content = "Hablar";
            }
        }
        private void BtnCortar_Click(object sender, RoutedEventArgs e)
        {
            if (_gestorActual is null)
                return;

            _sesionRepository.CerrarSesion(_gestorActual.IdSesion, _gestorActual.CalcularPuntajePromedio());



            _gestorActual = null;
            BtnCortar.IsEnabled = false;
            BtnProbarMicrofono.Content = "Iniciar conversacion";

            MessageBox.Show("Sesion guardada.");
        }
    }
}