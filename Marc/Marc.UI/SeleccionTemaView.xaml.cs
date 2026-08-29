using System.Windows;
using System.Windows.Controls;
using Marc.Core;
using Marc.Data;

namespace Marc.UI;

public partial class SeleccionTemaView : UserControl
{
    private readonly TemaRepository _temaRepository = new();

    public SeleccionTemaView()
    {
        InitializeComponent();
        ListaTemas.ItemsSource = _temaRepository.ObtenerTodos();
    }

    private void ListaTemas_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (ListaTemas.SelectedItem is not Tema temaSeleccionado)
            return;

        var mainWindow = (MainWindow)Window.GetWindow(this)!;
        mainWindow.IniciarLlamada(temaSeleccionado);
    }
}

