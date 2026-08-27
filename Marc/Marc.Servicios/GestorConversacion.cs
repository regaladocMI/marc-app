using Marc.Core;
using Marc.Data;

namespace Marc.Servicios;

public class ResultadoTurno
{
    public string TextoUsuario { get; set; } = string.Empty;
    public RespuestaTutor RespuestaAda { get; set; } = new();
    public string ProveedorUsado { get; set; } = string.Empty;
}

public class GestorConversacion
{
    private readonly ITranscriptorVoz _transcriptor;
    private readonly ITutorIA _tutor;
    private readonly ITutorIA? _tutorRespaldo;
    private readonly ISintetizadorVoz _sintetizador;
    private readonly MensajeRepository _mensajeRepository = new();
    private readonly CorreccionRepository _correccionRepository = new();
    private readonly VocabularioRepository _vocabularioRepository = new();

    private readonly List<(string Autor, string Texto)> _historial = new();
    private readonly List<int> _puntajes = new();
    private int _siguienteOrden = 1;

    public int IdSesion { get; }
    public string NombreUsuario { get; }
    public string NivelIngles { get; }
    public string NombreTema { get; }
    public string PromptBaseTema { get; }

    //lllllllllllllllllllllllllllllllllllllllllllllllllllllllllllllll
    public GestorConversacion(
    int idSesion,
    string nombreUsuario,
    string nivelIngles,
    string nombreTema,
    string promptBaseTema,
    ITranscriptorVoz transcriptor,
    ITutorIA tutor,
    ISintetizadorVoz sintetizador,
    ITutorIA? tutorRespaldo = null)
    {
        IdSesion = idSesion;
        NombreUsuario = nombreUsuario;
        NivelIngles = nivelIngles;
        NombreTema = nombreTema;
        PromptBaseTema = promptBaseTema;
        _transcriptor = transcriptor;
        _tutor = tutor;
        _sintetizador = sintetizador;
        _tutorRespaldo = tutorRespaldo;
    }

    public GestorConversacion(
        int idSesion,
        string nombreUsuario,
        string nivelIngles,
        string nombreTema,
        string promptBaseTema,
        ITranscriptorVoz transcriptor,
        ITutorIA tutor,
        ISintetizadorVoz sintetizador)
    {
        IdSesion = idSesion;
        NombreUsuario = nombreUsuario;
        NivelIngles = nivelIngles;
        NombreTema = nombreTema;
        PromptBaseTema = promptBaseTema;
        _transcriptor = transcriptor;
        _tutor = tutor;
        _sintetizador = sintetizador;
    }

    private async Task<(RespuestaTutor Respuesta, string Proveedor)> ObtenerRespuestaConRespaldoAsync(string textoUsuario)
    {
        try
        {
            var respuesta = await _tutor.ObtenerRespuestaAsync(NombreUsuario, NivelIngles, NombreTema, PromptBaseTema, _historial, textoUsuario);
            return (respuesta, "Gemini");
        }
        catch (Exception ex) when (_tutorRespaldo is not null && (ex is InvalidOperationException or TaskCanceledException))
        {
            var respuesta = await _tutorRespaldo.ObtenerRespuestaAsync(NombreUsuario, NivelIngles, NombreTema, PromptBaseTema, _historial, textoUsuario);
            return (respuesta, "Groq");
        }
    }

    public async Task<ResultadoTurno?> EjecutarTurnoAsync()
    {
        string textoUsuario = await _transcriptor.EscucharYTranscribirAsync();

        if (string.IsNullOrWhiteSpace(textoUsuario))
            return null;

        int idMensajeUsuario = _mensajeRepository.Insertar(
            IdSesion, MensajeRepository.ID_TIPO_EMISOR_USUARIO, textoUsuario, _siguienteOrden++, puntaje: null);

        var (respuesta, proveedorUsado) = await ObtenerRespuestaConRespaldoAsync(textoUsuario);

        _historial.Add(("Usuario", textoUsuario));
        _historial.Add(("Tutor", respuesta.RespuestaTexto));
        _puntajes.Add(respuesta.Puntaje);

        int idMensajeTutor = _mensajeRepository.Insertar(
            IdSesion, MensajeRepository.ID_TIPO_EMISOR_TUTOR, respuesta.RespuestaTexto, _siguienteOrden++, respuesta.Puntaje);

        if (respuesta.Correccion.HuboError)
        {
            _correccionRepository.Insertar(
                idMensajeUsuario,
                respuesta.Correccion.TipoError ?? "Uso y contexto",
                respuesta.Correccion.TextoOriginal ?? textoUsuario,
                respuesta.Correccion.TextoCorregido ?? textoUsuario,
                respuesta.Correccion.Explicacion);
        }

        foreach (var item in respuesta.VocabularioDetectado)
            _vocabularioRepository.GuardarSiNoExiste(item.PalabraOFrase, item.Significado);

        await _sintetizador.ReproducirAsync(respuesta.RespuestaTexto);

        return new ResultadoTurno { TextoUsuario = textoUsuario, RespuestaAda = respuesta, ProveedorUsado = proveedorUsado };
    }

    public decimal? CalcularPuntajePromedio()
        => _puntajes.Count > 0 ? (decimal)_puntajes.Average() : null;
}