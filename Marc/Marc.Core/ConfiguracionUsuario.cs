namespace Marc.Core;

public class ConfiguracionUsuario
{
    public int IdConfiguracionUsuario { get; set; }
    public int IdUsuario { get; set; }
    public int IdModoConversacion { get; set; }
    public decimal VelocidadHabla { get; set; }
    public decimal PacienciaSegundos { get; set; }
    public bool OcultarTranscripcion { get; set; }
    public string? MicrofonoPreferido { get; set; }
    public string? SalidaAudioPreferida { get; set; }
}