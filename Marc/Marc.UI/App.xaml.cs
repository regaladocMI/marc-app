using System.Windows;
using System.Windows.Threading;
using Marc.Core;
using Marc.Data;

namespace Marc.UI;

public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        DispatcherUnhandledException += App_DispatcherUnhandledException;

        var repositorio = new ConfiguracionUsuarioRepository();
        var configuracion = repositorio.ObtenerOCrear();

        ConfiguracionConversacion.VelocidadHabla = (double)configuracion.VelocidadHabla;
        ConfiguracionConversacion.PacienciaSegundos = (int)configuracion.PacienciaSegundos;
    }

    private void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
    {
        MessageBox.Show(
            "Ocurrio un problema inesperado:\n\n" +
            $"{e.Exception.Message}\n\n" +
            "La aplicacion va a continuar funcionando, pero si el problema persiste, cerra y volve a abrir MARC.",
            "MARC - Error",
            MessageBoxButton.OK,
            MessageBoxImage.Warning);

        e.Handled = true;
    }
}