using Microsoft.Data.SqlClient;
using Marc.Core;

namespace Marc.Data;

public class MensajeRepository
{
    public const int ID_TIPO_EMISOR_USUARIO = 1;
    public const int ID_TIPO_EMISOR_TUTOR = 2;

    public int Insertar(int idSesion, int idTipoEmisor, string texto, int orden, int? puntaje)
    {
        using var conexion = new SqlConnection(ConfiguracionApp.ObtenerCadenaConexion());
        conexion.Open();

        const string sql = @"
            INSERT INTO marc.mensaje (id_sesion, id_tipo_emisor, texto, orden, puntaje)
            OUTPUT INSERTED.id_mensaje
            VALUES (@idSesion, @idTipoEmisor, @texto, @orden, @puntaje)";

        using var comando = new SqlCommand(sql, conexion);
        comando.Parameters.AddWithValue("@idSesion", idSesion);
        comando.Parameters.AddWithValue("@idTipoEmisor", idTipoEmisor);
        comando.Parameters.AddWithValue("@texto", texto);
        comando.Parameters.AddWithValue("@orden", orden);
        comando.Parameters.AddWithValue("@puntaje", (object?)puntaje ?? DBNull.Value);

        return (int)comando.ExecuteScalar();
    }
}