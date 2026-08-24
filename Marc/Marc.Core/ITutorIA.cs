namespace Marc.Core;

public interface ITutorIA
{
    Task<RespuestaTutor> ObtenerRespuestaAsync(
        string nombreUsuario,
        string nivelIngles,
        string nombreTema,
        string promptBaseTema,
        List<(string Autor, string Texto)> historialConversacion,
        string mensajeUsuario);
}