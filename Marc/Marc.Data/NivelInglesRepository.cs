using Microsoft.Data.SqlClient;
using Marc.Core;

namespace Marc.Data;

public class NivelInglesRepository
{
    public List<NivelIngles> ObtenerTodos()
    {
        var niveles = new List<NivelIngles>();

        using var conexion = new SqlConnection(ConfiguracionApp.ObtenerCadenaConexion());
        conexion.Open();

        const string sql = "SELECT id_nivel_ingles, codigo, descripcion FROM marc.nivel_ingles ORDER BY id_nivel_ingles";

        using var comando = new SqlCommand(sql, conexion);
        using SqlDataReader lector = comando.ExecuteReader();

        while (lector.Read())
        {
            niveles.Add(new NivelIngles
            {
                IdNivelIngles = lector.GetInt32(lector.GetOrdinal("id_nivel_ingles")),
                Codigo = lector.GetString(lector.GetOrdinal("codigo")),
                Descripcion = lector.GetString(lector.GetOrdinal("descripcion"))
            });
        }

        return niveles;
    }
}