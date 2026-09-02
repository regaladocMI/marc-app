using System.Windows;
using Marc.Core;
using Marc.Data;

namespace Marc.UI;

public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        var repositorio = new ConfiguracionUsuarioRepository();
        var configuracion = repositorio.ObtenerOCrear();

        ConfiguracionConversacion.VelocidadHabla = (double)configuracion.VelocidadHabla;
        ConfiguracionConversacion.PacienciaSegundos = (int)configuracion.PacienciaSegundos;
    }
}