using System.IO;
using System.Text.Json;

namespace Marc.Data;

public static class ConfiguracionApp
{
    private static string? _cadenaConexion;

    public static string ObtenerCadenaConexion()
    {
        if (_cadenaConexion is not null)
            return _cadenaConexion;

        string ruta = Path.Combine(AppContext.BaseDirectory, "appsettings.local.json");

        if (!File.Exists(ruta))
            throw new FileNotFoundException(
                "Falta el archivo appsettings.local.json. Copialo en Marc.UI con tu cadena de conexion.");

        string contenidoJson = File.ReadAllText(ruta);
        using JsonDocument documento = JsonDocument.Parse(contenidoJson);

        _cadenaConexion = documento.RootElement
            .GetProperty("ConnectionStrings")
            .GetProperty("MarcDB")
            .GetString();

        return _cadenaConexion!;
    }
}