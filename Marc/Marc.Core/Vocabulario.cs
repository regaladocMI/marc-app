namespace Marc.Core;

public class Vocabulario
{
    public int IdVocabulario { get; set; }
    public int IdUsuario { get; set; }
    public string PalabraOFrase { get; set; } = string.Empty;
    public string? Significado { get; set; }
    public DateTime FechaCreacion { get; set; }
}