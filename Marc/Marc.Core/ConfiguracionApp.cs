using System.IO;
using System.Text.Json;

namespace Marc.Core;

public static class ConfiguracionApp
{
    private static string? _cadenaConexion;

    public static string ObtenerCadenaConexion()
    {
        if (_cadenaConexion is not null)
            return _cadenaConexion;

        _cadenaConexion = ObtenerRaizJson()
            .GetProperty("ConnectionStrings")
            .GetProperty("MarcDB")
            .GetString();

        return _cadenaConexion!;
    }

    private static JsonDocument? _documentoCacheado;

    private static JsonElement ObtenerRaizJson()
    {
        if (_documentoCacheado is not null)
            return _documentoCacheado.RootElement;

        string ruta = Path.Combine(AppContext.BaseDirectory, "appsettings.local.json");
        string contenidoJson = File.ReadAllText(ruta);
        _documentoCacheado = JsonDocument.Parse(contenidoJson);

        return _documentoCacheado.RootElement;
    }

    public static string ObtenerAzureSpeechKey()
        => ObtenerRaizJson().GetProperty("Azure").GetProperty("SpeechKey").GetString()!;

    public static string ObtenerAzureSpeechRegion()
        => ObtenerRaizJson().GetProperty("Azure").GetProperty("SpeechRegion").GetString()!;

    public static string ObtenerGeminiApiKey()
    => ObtenerRaizJson().GetProperty("Gemini").GetProperty("ApiKey").GetString()!;

    public static string ObtenerGroqApiKey()
    => ObtenerRaizJson().GetProperty("Groq").GetProperty("ApiKey").GetString()!;
}