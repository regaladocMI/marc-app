using System.Windows;
using System.Windows.Controls;
using Marc.Core;
using Marc.Data;

namespace Marc.UI;

public partial class TemasView : UserControl
{
    private readonly TemaRepository _temaRepository = new();
    private readonly NivelInglesRepository _nivelInglesRepository = new();
    private Tema? _temaSeleccionado;

    public TemasView()
    {
        InitializeComponent();
        CargarNiveles();
        CargarTemas();
    }

    private void CargarNiveles()
    {
        CmbNivel.ItemsSource = _nivelInglesRepository.ObtenerTodos();
    }

    private void CargarTemas()
    {
        bool incluirInactivos = ChkMostrarInactivos.IsChecked ?? false;
        ListaTemas.ItemsSource = _temaRepository.ObtenerTodos(incluirInactivos);
    }

    private void ChkMostrarInactivos_Changed(object sender, RoutedEventArgs e)
    {
        CargarTemas();
    }

    private void ListaTemas_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        _temaSeleccionado = ListaTemas.SelectedItem as Tema;

        if (_temaSeleccionado is null)
            return;

        TxtNombre.Text = _temaSeleccionado.Nombre;
        TxtPromptBase.Text = _temaSeleccionado.PromptBase;
        CmbNivel.SelectedValue = _temaSeleccionado.IdNivelIngles;
        ChkActivo.IsChecked = _temaSeleccionado.Activo;
    }

    private void BtnNuevo_Click(object sender, RoutedEventArgs e)
    {
        _temaSeleccionado = null;
        ListaTemas.SelectedItem = null;
        TxtNombre.Clear();
        TxtPromptBase.Clear();
        CmbNivel.SelectedIndex = -1;
        ChkActivo.IsChecked = true;
    }

    private void BtnGuardar_Click(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(TxtNombre.Text) || CmbNivel.SelectedValue is null)
        {
            MessageBox.Show("Nombre y nivel de ingles son obligatorios.");
            return;
        }

        var tema = _temaSeleccionado ?? new Tema();
        tema.Nombre = TxtNombre.Text;
        tema.PromptBase = TxtPromptBase.Text;
        tema.IdNivelIngles = (int)CmbNivel.SelectedValue;
        tema.Activo = ChkActivo.IsChecked ?? true;

        if (_temaSeleccionado is null)
            _temaRepository.Insertar(tema);
        else
            _temaRepository.Actualizar(tema);

        CargarTemas();
        BtnNuevo_Click(sender, e);
    }

    private void BtnEliminar_Click(object sender, RoutedEventArgs e)
    {
        if (_temaSeleccionado is null)
        {
            MessageBox.Show("Seleccioná un tema para eliminar.");
            return;
        }

        try
        {
            _temaRepository.Eliminar(_temaSeleccionado.IdTema);
            CargarTemas();
            BtnNuevo_Click(sender, e);
        }
        catch (InvalidOperationException ex)
        {
            MessageBox.Show(ex.Message);
        }
    }
}