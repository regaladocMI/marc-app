using Microsoft.Data.SqlClient;
using Marc.Core;

namespace Marc.Data;

public class SesionRepository
{
    private const int ID_USUARIO_FIJO = 1;

    public int IniciarSesion(int idTema)
    {
        using var conexion = new SqlConnection(ConfiguracionApp.ObtenerCadenaConexion());
        conexion.Open();

        const string sql = @"
            INSERT INTO marc.sesion (id_usuario, id_tema)
            OUTPUT INSERTED.id_sesion
            VALUES (@idUsuario, @idTema)";

        using var comando = new SqlCommand(sql, conexion);
        comando.Parameters.AddWithValue("@idUsuario", ID_USUARIO_FIJO);
        comando.Parameters.AddWithValue("@idTema", idTema);

        return (int)comando.ExecuteScalar();
    }

    public void CerrarSesion(int idSesion, decimal? puntajePromedio)
    {
        using var conexion = new SqlConnection(ConfiguracionApp.ObtenerCadenaConexion());
        conexion.Open();

        const string sql = @"
            UPDATE marc.sesion
            SET fecha_fin = SYSDATETIME(),
                puntaje_promedio = @puntajePromedio
            WHERE id_sesion = @idSesion";

        using var comando = new SqlCommand(sql, conexion);
        comando.Parameters.AddWithValue("@puntajePromedio", (object?)puntajePromedio ?? DBNull.Value);
        comando.Parameters.AddWithValue("@idSesion", idSesion);

        comando.ExecuteNonQuery();
    }
}