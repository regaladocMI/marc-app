namespace Marc.Core;

public class Correccion
{
    public int IdCorreccion { get; set; }
    public int IdMensaje { get; set; }
    public int IdTipoError { get; set; }
    public string TextoOriginal { get; set; } = string.Empty;
    public string TextoCorregido { get; set; } = string.Empty;
    public string? Explicacion { get; set; }
}