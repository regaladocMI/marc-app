namespace Marc.Core;

public class Sesion
{
    public int IdSesion { get; set; }
    public int IdUsuario { get; set; }
    public int IdTema { get; set; }
    public DateTime FechaInicio { get; set; }
    public DateTime? FechaFin { get; set; }
    public decimal? PuntajePromedio { get; set; }
}