using Microsoft.Data.SqlClient;
using Marc.Core;
namespace Marc.Data;

public class VocabularioRepository
{
    private const int ID_USUARIO_FIJO = 1;

    public void RegistrarOcurrencia(string palabraOFrase, string? significado, int idMensaje)
    {
        using var conexion = new SqlConnection(ConfiguracionApp.ObtenerCadenaConexion());
        conexion.Open();

        int idVocabulario = ObtenerOCrearVocabulario(conexion, palabraOFrase, significado);

        const string sqlOcurrencia = @"
            INSERT INTO marc.vocabulario_ocurrencia (id_vocabulario, id_mensaje)
            VALUES (@idVocabulario, @idMensaje)";

        using var comandoOcurrencia = new SqlCommand(sqlOcurrencia, conexion);
        comandoOcurrencia.Parameters.AddWithValue("@idVocabulario", idVocabulario);
        comandoOcurrencia.Parameters.AddWithValue("@idMensaje", idMensaje);
        comandoOcurrencia.ExecuteNonQuery();
    }

    private static int ObtenerOCrearVocabulario(SqlConnection conexion, string palabraOFrase, string? significado)
    {
        const string sqlBuscar = @"
            SELECT id_vocabulario FROM marc.vocabulario
            WHERE id_usuario = @idUsuario AND palabra_o_frase = @palabra";

        using (var comandoBuscar = new SqlCommand(sqlBuscar, conexion))
        {
            comandoBuscar.Parameters.AddWithValue("@idUsuario", ID_USUARIO_FIJO);
            comandoBuscar.Parameters.AddWithValue("@palabra", palabraOFrase);

            object? resultado = comandoBuscar.ExecuteScalar();
            if (resultado is not null)
                return (int)resultado;
        }

        const string sqlInsertar = @"
            INSERT INTO marc.vocabulario (id_usuario, palabra_o_frase, significado)
            OUTPUT INSERTED.id_vocabulario
            VALUES (@idUsuario, @palabra, @significado)";

        using var comandoInsertar = new SqlCommand(sqlInsertar, conexion);
        comandoInsertar.Parameters.AddWithValue("@idUsuario", ID_USUARIO_FIJO);
        comandoInsertar.Parameters.AddWithValue("@palabra", palabraOFrase);
        comandoInsertar.Parameters.AddWithValue("@significado", (object?)significado ?? DBNull.Value);

        return (int)comandoInsertar.ExecuteScalar();
    }
}