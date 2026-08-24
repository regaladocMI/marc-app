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
using Marc.Core;
using Marc.Servicios;

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

        private async void BtnProbarMicrofono_Click(object sender, RoutedEventArgs e)
        {
            ITranscriptorVoz transcriptor = new TranscriptorAzure();
            ITutorIA tutor = new TutorGemini();

            MessageBox.Show("Habla algo en ingles ahora.");
            string textoTranscrito = await transcriptor.EscucharYTranscribirAsync();

            if (string.IsNullOrEmpty(textoTranscrito))
            {
                MessageBox.Show("No se detecto ninguna frase.");
                return;
            }

            BtnProbarMicrofono.IsEnabled = false;
            BtnProbarMicrofono.Content = "Ada esta pensando...";

            try
            {
                var cronometro = System.Diagnostics.Stopwatch.StartNew();

                RespuestaTutor respuesta = await tutor.ObtenerRespuestaAsync(
                    nombreUsuario: "Alex",
                    nivelIngles: "B1",
                    nombreTema: "Conversacion libre",
                    promptBaseTema: "Charla informal de practica.",
                    historialConversacion: new List<(string, string)>(),
                    mensajeUsuario: textoTranscrito);

                long msGemini = cronometro.ElapsedMilliseconds;

                ISintetizadorVoz sintetizador = new SintetizadorAzure();
                await sintetizador.ReproducirAsync(respuesta.RespuestaTexto);

                long msTotal = cronometro.ElapsedMilliseconds;
                System.Diagnostics.Debug.WriteLine($"Gemini: {msGemini}ms | Azure TTS: {msTotal - msGemini}ms | Total: {msTotal}ms");

                MessageBox.Show(
                    $"Vos dijiste: {textoTranscrito}\n\n" +
                    $"Ada ({respuesta.Puntaje}/10): {respuesta.RespuestaTexto}\n\n" +
                    (respuesta.Correccion.HuboError
                        ? $"Correccion: \"{respuesta.Correccion.TextoOriginal}\" -> \"{respuesta.Correccion.TextoCorregido}\"\n{respuesta.Correccion.Explicacion}"
                        : "Sin errores."));
            }
            catch (InvalidOperationException)
            {
                MessageBox.Show("Ada esta tardando mas de lo normal en responder (el servicio esta saturado). Intenta de nuevo en un momento.");
            }
            finally
            {
                BtnProbarMicrofono.IsEnabled = true;
                BtnProbarMicrofono.Content = "Probar microfono";
            }
        }
    }
}