using Marc.Core;
using Microsoft.Data.SqlClient;

namespace Marc.Data;

public class VocabularioRepository
{
    private const int ID_USUARIO_FIJO = 1;

    public void GuardarSiNoExiste(string palabraOFrase, string? significado)
    {
        using var conexion = new SqlConnection(ConfiguracionApp.ObtenerCadenaConexion());
        conexion.Open();

        const string sql = @"
            IF NOT EXISTS (
                SELECT 1 FROM marc.vocabulario
                WHERE id_usuario = @idUsuario AND palabra_o_frase = @palabra
            )
            INSERT INTO marc.vocabulario (id_usuario, palabra_o_frase, significado)
            VALUES (@idUsuario, @palabra, @significado)";

        using var comando = new SqlCommand(sql, conexion);
        comando.Parameters.AddWithValue("@idUsuario", ID_USUARIO_FIJO);
        comando.Parameters.AddWithValue("@palabra", palabraOFrase);
        comando.Parameters.AddWithValue("@significado", (object?)significado ?? DBNull.Value);

        comando.ExecuteNonQuery();
    }
}