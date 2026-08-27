using System.Text;
using System.Text.Json;
using Marc.Core;

namespace Marc.Servicios;

public class TutorGroq : ITutorIA
{
    private readonly HttpClient _http = new() { Timeout = TimeSpan.FromSeconds(5) };

    public async Task<RespuestaTutor> ObtenerRespuestaAsync(
        string nombreUsuario,
        string nivelIngles,
        string nombreTema,
        string promptBaseTema,
        List<(string Autor, string Texto)> historialConversacion,
        string mensajeUsuario)
    {
        string instruccionSistema = ConstructorPromptAda.Construir(nombreUsuario, nivelIngles, nombreTema, promptBaseTema);

        var mensajes = new List<object>
        {
            new { role = "system", content = instruccionSistema }
        };

        foreach (var turno in historialConversacion)
        {
            mensajes.Add(new
            {
                role = turno.Autor == "Usuario" ? "user" : "assistant",
                content = turno.Texto
            });
        }

        mensajes.Add(new { role = "user", content = mensajeUsuario });

        var cuerpoSolicitud = new
        {
            model = "openai/gpt-oss-120b",
            messages = mensajes,
            response_format = new { type = "json_object" }
        };

        var contenidoHttp = new StringContent(JsonSerializer.Serialize(cuerpoSolicitud), Encoding.UTF8, "application/json");
        _http.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", ConfiguracionApp.ObtenerGroqApiKey());

        HttpResponseMessage respuesta = await _http.PostAsync("https://api.groq.com/openai/v1/chat/completions", contenidoHttp);
        string textoRespuesta = await respuesta.Content.ReadAsStringAsync();

        if (!respuesta.IsSuccessStatusCode)
            throw new InvalidOperationException($"Error de Groq ({respuesta.StatusCode}): {textoRespuesta}");

        using JsonDocument documento = JsonDocument.Parse(textoRespuesta);
        string jsonDeAda = documento.RootElement
            .GetProperty("choices")[0]
            .GetProperty("message")
            .GetProperty("content")
            .GetString()!;

        return JsonSerializer.Deserialize<RespuestaTutor>(jsonDeAda)
            ?? throw new InvalidOperationException("Groq devolvio un JSON vacio o invalido.");
    }
}