using System.Text;
using System.Text.Json;
using Marc.Core;

namespace Marc.Servicios;

public class TutorGemini : ITutorIA
{
    private readonly HttpClient _http = new() { Timeout = TimeSpan.FromSeconds(10) };
    public async Task<RespuestaTutor> ObtenerRespuestaAsync(
    string nombreUsuario,
    string nivelIngles,
    string nombreTema,
    string promptBaseTema,
    List<(string Autor, string Texto)> historialConversacion,
    string mensajeUsuario)
    {
        string instruccionSistema = ConstructorPromptAda.Construir(nombreUsuario, nivelIngles, nombreTema, promptBaseTema);

        var contenidos = new List<object>();

        foreach (var turno in historialConversacion)
        {
            contenidos.Add(new
            {
                role = turno.Autor == "Usuario" ? "user" : "model",
                parts = new[] { new { text = turno.Texto } }
            });
        }

        contenidos.Add(new
        {
            role = "user",
            parts = new[] { new { text = mensajeUsuario } }
        });

        var cuerpoSolicitud = new
        {
            system_instruction = new { parts = new[] { new { text = instruccionSistema } } },
            contents = contenidos,
            generationConfig = new { responseMimeType = "application/json" }
        };

        string url = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-flash-latest:generateContent?key={ConfiguracionApp.ObtenerGeminiApiKey()}"; string textoRespuesta = await LlamarConReintentosAsync(url, cuerpoSolicitud);

        using JsonDocument documento = JsonDocument.Parse(textoRespuesta);
        string jsonDeAda = documento.RootElement
            .GetProperty("candidates")[0]
            .GetProperty("content")
            .GetProperty("parts")[0]
            .GetProperty("text")
            .GetString()!;

        return JsonSerializer.Deserialize<RespuestaTutor>(jsonDeAda)
            ?? throw new InvalidOperationException("Gemini devolvio un JSON vacio o invalido.");
    }

    private async Task<string> LlamarConReintentosAsync(string url, object cuerpoSolicitud)
    {
        const int maximoIntentos = 3;

        for (int intento = 1; intento <= maximoIntentos; intento++)
        {
            var contenidoHttp = new StringContent(JsonSerializer.Serialize(cuerpoSolicitud), Encoding.UTF8, "application/json");
            HttpResponseMessage respuesta = await _http.PostAsync(url, contenidoHttp);
            string textoRespuesta = await respuesta.Content.ReadAsStringAsync();

            if (respuesta.IsSuccessStatusCode)
                return textoRespuesta;

            bool esErrorTemporal = respuesta.StatusCode == System.Net.HttpStatusCode.ServiceUnavailable
                                 || respuesta.StatusCode == System.Net.HttpStatusCode.TooManyRequests;

            if (!esErrorTemporal || intento == maximoIntentos)
                throw new InvalidOperationException($"Error de Gemini ({respuesta.StatusCode}): {textoRespuesta}");

            await Task.Delay(1500 * intento);
        }

        throw new InvalidOperationException("No se pudo contactar a Gemini tras varios intentos.");
    }

    private static string ConstruirInstruccionSistema(string nombreUsuario, string nivelIngles, string nombreTema, string promptBaseTema)
    {
        return $$"""
            Sos Ada, una tutora de ingles conversacional calida y paciente, hablando
            con {{nombreUsuario}}, un estudiante hispanohablante de nivel {{nivelIngles}}.
            El tema de conversacion de hoy es: {{nombreTema}}. Contexto adicional: {{promptBaseTema}}

            Reglas de estilo:
            - Respondes siempre en ingles, salvo que el estudiante muestre confusion clara
              ("I don't understand", silencio, respuesta sin sentido) - ahi das UNA aclaracion
              corta en espanol y volves a ingles en la misma respuesta.
            - Mantene las respuestas cortas: 1 a 3 oraciones. Nunca clases de gramatica largas.
            - Si hay un error, corregilo con una explicacion breve (menos de 15 palabras) y
              pedile que repita la version corregida.
            - Si no hay error, reforza positivamente en una frase y segui la conversacion con
              una pregunta relacionada al tema.
            - Si el estudiante menciona un nombre propio, lugar, o palabra de vocabulario
              relevante, marcalo en vocabulario_detectado.
            - Nunca rompas el personaje ni menciones que sos una inteligencia artificial.

            Respondes UNICAMENTE con un JSON que cumpla exactamente este esquema, sin texto
            adicional antes ni despues:
            {
              "respuesta_texto": "string",
              "puntaje": number del 1 al 10,
              "correccion": {
                "hubo_error": boolean,
                "texto_original": "string o null",
                "texto_corregido": "string o null",
                "explicacion": "string o null",
                "tipo_error": "Gramatica | Vocabulario | Pronunciacion | Uso y contexto | null"
              },
              "vocabulario_detectado": [
                { "palabra_o_frase": "string", "significado": "string breve" }
              ]
            }
            """;
    }
}