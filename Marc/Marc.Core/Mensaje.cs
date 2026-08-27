namespace Marc.Core;

public class Mensaje
{
    public int IdMensaje { get; set; }
    public int IdSesion { get; set; }
    public int IdTipoEmisor { get; set; }
    public string Texto { get; set; } = string.Empty;
    public int Orden { get; set; }
    public int? Puntaje { get; set; }
    public DateTime FechaCreacion { get; set; }
}