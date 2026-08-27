using Marc.Core;
using Microsoft.Data.SqlClient;

namespace Marc.Data;

public class CorreccionRepository
{
    public void Insertar(int idMensaje, string tipoError, string textoOriginal, string textoCorregido, string? explicacion)
    {
        using var conexion = new SqlConnection(ConfiguracionApp.ObtenerCadenaConexion());
        conexion.Open();

        int idTipoError = ObtenerIdTipoError(conexion, tipoError);

        const string sql = @"
            INSERT INTO marc.correccion (id_mensaje, id_tipo_error, texto_original, texto_corregido, explicacion)
            VALUES (@idMensaje, @idTipoError, @textoOriginal, @textoCorregido, @explicacion)";

        using var comando = new SqlCommand(sql, conexion);
        comando.Parameters.AddWithValue("@idMensaje", idMensaje);
        comando.Parameters.AddWithValue("@idTipoError", idTipoError);
        comando.Parameters.AddWithValue("@textoOriginal", textoOriginal);
        comando.Parameters.AddWithValue("@textoCorregido", textoCorregido);
        comando.Parameters.AddWithValue("@explicacion", (object?)explicacion ?? DBNull.Value);

        comando.ExecuteNonQuery();
    }

    private static int ObtenerIdTipoError(SqlConnection conexion, string nombreTipoError)
    {
        const string sql = "SELECT id_tipo_error FROM marc.tipo_error WHERE nombre = @nombre";

        using var comando = new SqlCommand(sql, conexion);
        comando.Parameters.AddWithValue("@nombre", nombreTipoError);

        object? resultado = comando.ExecuteScalar();

        return resultado is not null
            ? (int)resultado
            : throw new InvalidOperationException($"Tipo de error desconocido: '{nombreTipoError}'. Verificar que Ada devuelva uno de los valores de marc.tipo_error.");
    }
}