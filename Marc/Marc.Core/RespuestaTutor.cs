using System.Text.Json.Serialization;

namespace Marc.Core;

public class RespuestaTutor
{
    [JsonPropertyName("respuesta_texto")]
    public string RespuestaTexto { get; set; } = string.Empty;

    [JsonPropertyName("puntaje")]
    public int Puntaje { get; set; }

    [JsonPropertyName("correccion")]
    public CorreccionInfo Correccion { get; set; } = new();

    [JsonPropertyName("vocabulario_detectado")]
    public List<VocabularioItem> VocabularioDetectado { get; set; } = new();
}

public class CorreccionInfo
{
    [JsonPropertyName("hubo_error")]
    public bool HuboError { get; set; }

    [JsonPropertyName("texto_original")]
    public string? TextoOriginal { get; set; }

    [JsonPropertyName("texto_corregido")]
    public string? TextoCorregido { get; set; }

    [JsonPropertyName("explicacion")]
    public string? Explicacion { get; set; }

    [JsonPropertyName("tipo_error")]
    public string? TipoError { get; set; }
}

public class VocabularioItem
{
    [JsonPropertyName("palabra_o_frase")]
    public string PalabraOFrase { get; set; } = string.Empty;

    [JsonPropertyName("significado")]
    public string Significado { get; set; } = string.Empty;
}