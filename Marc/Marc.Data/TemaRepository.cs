using Microsoft.Data.SqlClient;
using Marc.Core;

namespace Marc.Data;

public class TemaRepository
{
    private const int ID_USUARIO_FIJO = 1;

    public List<Tema> ObtenerTodos(bool incluirInactivos = false)
    {
        var temas = new List<Tema>();

        using var conexion = new SqlConnection(ConfiguracionApp.ObtenerCadenaConexion());
        conexion.Open();

        string sql = @"
        SELECT id_tema, id_usuario, id_nivel_ingles, nombre, prompt_base, activo, fecha_creacion
        FROM marc.tema
        WHERE id_usuario = @idUsuario";

        if (!incluirInactivos)
            sql += " AND activo = 1";

        sql += " ORDER BY nombre";

        using var comando = new SqlCommand(sql, conexion);
        comando.Parameters.AddWithValue("@idUsuario", ID_USUARIO_FIJO);

        using SqlDataReader lector = comando.ExecuteReader();
        while (lector.Read())
        {
            temas.Add(new Tema
            {
                IdTema = lector.GetInt32(lector.GetOrdinal("id_tema")),
                IdUsuario = lector.GetInt32(lector.GetOrdinal("id_usuario")),
                IdNivelIngles = lector.GetInt32(lector.GetOrdinal("id_nivel_ingles")),
                Nombre = lector.GetString(lector.GetOrdinal("nombre")),
                PromptBase = lector.GetString(lector.GetOrdinal("prompt_base")),
                Activo = lector.GetBoolean(lector.GetOrdinal("activo")),
                FechaCreacion = lector.GetDateTime(lector.GetOrdinal("fecha_creacion"))
            });
        }

        return temas;
    }

    public int Insertar(Tema tema)
    {
        using var conexion = new SqlConnection(ConfiguracionApp.ObtenerCadenaConexion());
        conexion.Open();

        const string sql = @"
            INSERT INTO marc.tema (id_usuario, id_nivel_ingles, nombre, prompt_base, activo)
            OUTPUT INSERTED.id_tema
            VALUES (@idUsuario, @idNivelIngles, @nombre, @promptBase, @activo)";

        using var comando = new SqlCommand(sql, conexion);
        comando.Parameters.AddWithValue("@idUsuario", ID_USUARIO_FIJO);
        comando.Parameters.AddWithValue("@idNivelIngles", tema.IdNivelIngles);
        comando.Parameters.AddWithValue("@nombre", tema.Nombre);
        comando.Parameters.AddWithValue("@promptBase", tema.PromptBase);
        comando.Parameters.AddWithValue("@activo", tema.Activo);

        int nuevoId = (int)comando.ExecuteScalar();
        return nuevoId;
    }

    public void Actualizar(Tema tema)
    {
        using var conexion = new SqlConnection(ConfiguracionApp.ObtenerCadenaConexion());
        conexion.Open();

        const string sql = @"
            UPDATE marc.tema
            SET id_nivel_ingles = @idNivelIngles,
                nombre = @nombre,
                prompt_base = @promptBase,
                activo = @activo
            WHERE id_tema = @idTema AND id_usuario = @idUsuario";

        using var comando = new SqlCommand(sql, conexion);
        comando.Parameters.AddWithValue("@idNivelIngles", tema.IdNivelIngles);
        comando.Parameters.AddWithValue("@nombre", tema.Nombre);
        comando.Parameters.AddWithValue("@promptBase", tema.PromptBase);
        comando.Parameters.AddWithValue("@activo", tema.Activo);
        comando.Parameters.AddWithValue("@idTema", tema.IdTema);
        comando.Parameters.AddWithValue("@idUsuario", ID_USUARIO_FIJO);

        comando.ExecuteNonQuery();
    }

    public void Eliminar(int idTema)
    {
        using var conexion = new SqlConnection(ConfiguracionApp.ObtenerCadenaConexion());
        conexion.Open();

        const string sql = "DELETE FROM marc.tema WHERE id_tema = @idTema AND id_usuario = @idUsuario";

        using var comando = new SqlCommand(sql, conexion);
        comando.Parameters.AddWithValue("@idTema", idTema);
        comando.Parameters.AddWithValue("@idUsuario", ID_USUARIO_FIJO);

        try
        {
            comando.ExecuteNonQuery();
        }
        catch (SqlException ex) when (ex.Number == 547)
        {
            throw new InvalidOperationException(
                "No se puede eliminar este tema porque tiene sesiones de conversacion asociadas.", ex);
        }
    }
}