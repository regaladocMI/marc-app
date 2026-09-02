using Microsoft.Data.SqlClient;
using Marc.Core;

namespace Marc.Data;

public class ConfiguracionUsuarioRepository
{
    private const int ID_USUARIO_FIJO = 1;
    private const int ID_MODO_VOZ = 1;

    public ConfiguracionUsuario ObtenerOCrear()
    {
        using var conexion = new SqlConnection(ConfiguracionApp.ObtenerCadenaConexion());
        conexion.Open();

        const string sqlBuscar = @"
            SELECT id_configuracion_usuario, id_usuario, id_modo_conversacion,
                   velocidad_habla, paciencia_segundos, ocultar_transcripcion,
                   microfono_preferido, salida_audio_preferida
            FROM marc.configuracion_usuario
            WHERE id_usuario = @idUsuario";

        using (var comandoBuscar = new SqlCommand(sqlBuscar, conexion))
        {
            comandoBuscar.Parameters.AddWithValue("@idUsuario", ID_USUARIO_FIJO);

            using SqlDataReader lector = comandoBuscar.ExecuteReader();
            if (lector.Read())
            {
                return new ConfiguracionUsuario
                {
                    IdConfiguracionUsuario = lector.GetInt32(lector.GetOrdinal("id_configuracion_usuario")),
                    IdUsuario = lector.GetInt32(lector.GetOrdinal("id_usuario")),
                    IdModoConversacion = lector.GetInt32(lector.GetOrdinal("id_modo_conversacion")),
                    VelocidadHabla = lector.GetDecimal(lector.GetOrdinal("velocidad_habla")),
                    PacienciaSegundos = lector.GetDecimal(lector.GetOrdinal("paciencia_segundos")),
                    OcultarTranscripcion = lector.GetBoolean(lector.GetOrdinal("ocultar_transcripcion")),
                    MicrofonoPreferido = lector.IsDBNull(lector.GetOrdinal("microfono_preferido")) ? null : lector.GetString(lector.GetOrdinal("microfono_preferido")),
                    SalidaAudioPreferida = lector.IsDBNull(lector.GetOrdinal("salida_audio_preferida")) ? null : lector.GetString(lector.GetOrdinal("salida_audio_preferida"))
                };
            }
        }

        return CrearPorDefecto(conexion);
    }

    private static ConfiguracionUsuario CrearPorDefecto(SqlConnection conexion)
    {
        const string sqlInsertar = @"
            INSERT INTO marc.configuracion_usuario (id_usuario, id_modo_conversacion, velocidad_habla, paciencia_segundos)
            OUTPUT INSERTED.id_configuracion_usuario
            VALUES (@idUsuario, @idModoConversacion, 1.00, 2.00)";

        using var comandoInsertar = new SqlCommand(sqlInsertar, conexion);
        comandoInsertar.Parameters.AddWithValue("@idUsuario", ID_USUARIO_FIJO);
        comandoInsertar.Parameters.AddWithValue("@idModoConversacion", ID_MODO_VOZ);

        int idNuevo = (int)comandoInsertar.ExecuteScalar();

        return new ConfiguracionUsuario
        {
            IdConfiguracionUsuario = idNuevo,
            IdUsuario = ID_USUARIO_FIJO,
            IdModoConversacion = ID_MODO_VOZ,
            VelocidadHabla = 1.00m,
            PacienciaSegundos = 2.00m,
            OcultarTranscripcion = false
        };
    }

    public void Actualizar(decimal velocidadHabla, decimal pacienciaSegundos)
    {
        using var conexion = new SqlConnection(ConfiguracionApp.ObtenerCadenaConexion());
        conexion.Open();

        const string sql = @"
            UPDATE marc.configuracion_usuario
            SET velocidad_habla = @velocidad, paciencia_segundos = @paciencia
            WHERE id_usuario = @idUsuario";

        using var comando = new SqlCommand(sql, conexion);
        comando.Parameters.AddWithValue("@velocidad", velocidadHabla);
        comando.Parameters.AddWithValue("@paciencia", pacienciaSegundos);
        comando.Parameters.AddWithValue("@idUsuario", ID_USUARIO_FIJO);

        comando.ExecuteNonQuery();
    }
}