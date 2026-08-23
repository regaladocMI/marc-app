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

            MessageBox.Show("Hablá algo en ingles ahora. Cuando dejes de hablar, se corta solo.");

            string textoTranscrito = await transcriptor.EscucharYTranscribirAsync();

            MessageBox.Show(string.IsNullOrEmpty(textoTranscrito)
                ? "No se detecto ninguna frase."
                : $"Transcripcion: {textoTranscrito}");
        }
    }


}