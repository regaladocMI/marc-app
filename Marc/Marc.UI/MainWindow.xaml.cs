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


        public void IniciarLlamada(Tema tema)
        {
            ContenedorPrincipal.Content = new LlamadaView(tema);
        }
        private void BtnTemas_Click(object sender, RoutedEventArgs e)
        {
            ContenedorPrincipal.Content = new TemasView();
        }

        private SesionRepository _sesionRepository = new();


        public void VolverAlMenuPrincipal()
        {
            ContenedorPrincipal.Content = null;
        }

        private void BtnPracticar_Click(object sender, RoutedEventArgs e)
        {
            ContenedorPrincipal.Content = new SeleccionTemaView();
        }

        private void BtnHistorial_Click(object sender, RoutedEventArgs e)
        {
            ContenedorPrincipal.Content = new HistorialView();
        }

        private void BtnReportes_Click(object sender, RoutedEventArgs e)
        {
            ContenedorPrincipal.Content = new ReportesView();
        }

        private void BtnVocabulario_Click(object sender, RoutedEventArgs e)
        {
            ContenedorPrincipal.Content = new VocabularioView();
        }
    }
}