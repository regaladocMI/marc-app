using Marc.Core;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Marc.Data;

public class ReporteRepository
{
    private const int ID_USUARIO_FIJO = 1;

    public DataTable ObtenerProgresoPorFecha(DateTime desde, DateTime hasta)
    {
        using var conexion = new SqlConnection(ConfiguracionApp.ObtenerCadenaConexion());

        const string sql = @"
            SELECT
                s.id_sesion,
                t.nombre AS Tema,
                s.fecha_inicio AS Fecha,
                s.fecha_fin AS Finalizada,
                s.puntaje_promedio AS Puntaje
            FROM marc.sesion s
            INNER JOIN marc.tema t ON t.id_tema = s.id_tema
            WHERE s.id_usuario = @idUsuario
              AND s.fecha_inicio >= @desde
              AND s.fecha_inicio < @hasta
            ORDER BY s.fecha_inicio DESC";

        using var adaptador = new SqlDataAdapter(sql, conexion);
        adaptador.SelectCommand.Parameters.AddWithValue("@idUsuario", ID_USUARIO_FIJO);
        adaptador.SelectCommand.Parameters.AddWithValue("@desde", desde.Date);
        adaptador.SelectCommand.Parameters.AddWithValue("@hasta", hasta.Date.AddDays(1));

        var tabla = new DataTable();
        adaptador.Fill(tabla);

        return tabla;
    }

    public DataTable ObtenerMensajesDeSesion(int idSesion)
    {
        using var conexion = new SqlConnection(ConfiguracionApp.ObtenerCadenaConexion());

        const string sql = @"
        SELECT
            te.nombre AS Autor,
            m.texto AS Mensaje,
            m.puntaje AS Puntaje,
            m.fecha_creacion AS Fecha
        FROM marc.mensaje m
        INNER JOIN marc.tipo_emisor te ON te.id_tipo_emisor = m.id_tipo_emisor
        WHERE m.id_sesion = @idSesion
        ORDER BY m.orden";

        using var adaptador = new SqlDataAdapter(sql, conexion);
        adaptador.SelectCommand.Parameters.AddWithValue("@idSesion", idSesion);

        var tabla = new DataTable();
        adaptador.Fill(tabla);

        return tabla;
    }


    public DataTable ObtenerPuntajePromedioPorTema(DateTime desde, DateTime hasta)
    {
        using var conexion = new SqlConnection(ConfiguracionApp.ObtenerCadenaConexion());

        const string sql = @"
        SELECT
            t.nombre AS Tema,
            COUNT(s.id_sesion) AS CantidadSesiones,
            AVG(s.puntaje_promedio) AS PromedioGeneral
        FROM marc.sesion s
        INNER JOIN marc.tema t ON t.id_tema = s.id_tema
        WHERE s.id_usuario = @idUsuario
          AND s.fecha_inicio >= @desde
          AND s.fecha_inicio < @hasta
          AND s.puntaje_promedio IS NOT NULL
        GROUP BY t.nombre
        ORDER BY PromedioGeneral DESC";

        using var adaptador = new SqlDataAdapter(sql, conexion);
        adaptador.SelectCommand.Parameters.AddWithValue("@idUsuario", ID_USUARIO_FIJO);
        adaptador.SelectCommand.Parameters.AddWithValue("@desde", desde.Date);
        adaptador.SelectCommand.Parameters.AddWithValue("@hasta", hasta.Date.AddDays(1));

        var tabla = new DataTable();
        adaptador.Fill(tabla);

        return tabla;
    }

    public DataTable ObtenerVocabularioMasRepetido(int topN)
    {
        using var conexion = new SqlConnection(ConfiguracionApp.ObtenerCadenaConexion());

        const string sql = @"
        SELECT TOP (@topN)
            v.palabra_o_frase AS Palabra,
            v.significado AS Significado,
            COUNT(vo.id_vocabulario_ocurrencia) AS VecesRepetida
        FROM marc.vocabulario v
        LEFT JOIN marc.vocabulario_ocurrencia vo ON vo.id_vocabulario = v.id_vocabulario
        WHERE v.id_usuario = @idUsuario
        GROUP BY v.palabra_o_frase, v.significado
        ORDER BY VecesRepetida DESC";

        using var adaptador = new SqlDataAdapter(sql, conexion);
        adaptador.SelectCommand.Parameters.AddWithValue("@topN", topN);
        adaptador.SelectCommand.Parameters.AddWithValue("@idUsuario", ID_USUARIO_FIJO);

        var tabla = new DataTable();
        adaptador.Fill(tabla);

        return tabla;
    }

}