using System.Globalization; // permite CultureInfo
using Microsoft.Data.Sqlite;

public class VueloRepository(string cadenaConexion)
{
    public const string FormatoFecha = "yyyy-MM-dd HH:mm";
    public List<Vuelo> Listar()
    {
        using var conexion = new SqliteConnection(cadenaConexion);
        conexion.Open();

        var sql = """
            SELECT v.Id, v.Numero, v.Salida, v.DuracionMinutos, v.Filas, v.Precio,
                   o.Id, o.Codigo, o.Ciudad, o.Nombre,
                   d.Id, d.Codigo, d.Ciudad, d.Nombre
            FROM Vuelo v
            INNER JOIN Aeropuerto o ON v.OrigenId  = o.Id
            INNER JOIN Aeropuerto d ON v.DestinoId = d.Id
            ORDER BY v.Salida
            """;

        using var comando = new SqliteCommand(sql, conexion);
        return LeerVuelos(comando);
    }

    public Vuelo? Obtener(int id)
    {
        using var conexion = new SqliteConnection(cadenaConexion);
        conexion.Open();

        var sql = """
    SELECT v.Id, v.Numero, v.Salida, v.DuracionMinutos, v.Filas, v.Precio,
           o.Id, o.Codigo, o.Ciudad, o.Nombre,
           d.Id, d.Codigo, d.Ciudad, d.Nombre
    FROM Vuelo v
    INNER JOIN Aeropuerto o ON v.OrigenId  = o.Id
    INNER JOIN Aeropuerto d ON v.DestinoId = d.Id
    WHERE v.Id = @id
    """;;

        using var comando = new SqliteCommand(sql, conexion);
        comando.Parameters.AddWithValue("@id", id);
        using var lector = comando.ExecuteReader();
        return lector.Read() ? LeerVuelo(lector, id) : null;
    }

    public List<OcupacionVuelo> ListarOcupacion()
    {
        List<OcupacionVuelo> vuelosO = new List<OcupacionVuelo>();

        using var conexion = new SqliteConnection(cadenaConexion);
        conexion.Open();

        var sql = """
            SELECT v.Id, v.Numero, o.Codigo || ' → ' || d.Codigo AS Ruta, v.Salida,
                   v.Filas * 6 AS Capacidad, COUNT(r.Id) AS Ocupados
            FROM Vuelo v
            INNER JOIN Aeropuerto o ON v.OrigenId  = o.Id
            INNER JOIN Aeropuerto d ON v.DestinoId = d.Id
            LEFT JOIN Reserva r ON r.VueloId = v.Id AND r.Estado = 'Confirmada'
            WHERE v.Salida >= @ahora
            GROUP BY v.Id, v.Numero, Ruta, v.Salida, v.Filas
            ORDER BY v.Salida
            """;
        using var comando = new SqliteCommand(sql, conexion);
        comando.Parameters.AddWithValue("@ahora", DateTime.Now.ToString(FormatoFecha, CultureInfo.InvariantCulture));

        var resultado = new List<OcupacionVuelo>();
        using var lector = comando.ExecuteReader();
        while (lector.Read())
        {
            resultado.Add(new OcupacionVuelo(
                lector.GetInt32(0),
                lector.GetString(1),
                lector.GetString(2),
                LeerFecha(lector, 3),
                lector.GetInt32(4),
                lector.GetInt32(5)));
        }
        return resultado;
    }

    public List<Vuelo> Buscar(int? destinoId, DateTime? desde)
    {
        using var conexion = new SqliteConnection(cadenaConexion);
        conexion.Open();

        var sql = """
            SELECT v.Id, v.Numero, v.Salida, v.DuracionMinutos, v.Filas, v.Precio,
                   o.Id, o.Codigo, o.Ciudad, o.Nombre,
                   d.Id, d.Codigo, d.Ciudad, d.Nombre
            FROM Vuelo v
            INNER JOIN Aeropuerto o ON v.OrigenId  = o.Id
            INNER JOIN Aeropuerto d ON v.DestinoId = d.Id
            WHERE d.Id = @destinoId AND v.Salida >= @desde 
            ORDER BY v.Salida
            """;
        using var comando = new SqliteCommand(sql, conexion);
        comando.Parameters.AddWithValue("@destinoId",destinoId);
        comando.Parameters.AddWithValue("@desde",desde);
        return LeerVuelos(comando);
    }

        public bool ExisteNumero(string numero)
        {
            using var conexion = new SqliteConnection(cadenaConexion);
            conexion.Open();

            var sql = "SELECT COUNT(*) FROM Vuelo WHERE Numero = @numero";
            using var comando = new SqliteCommand(sql, conexion);
            comando.Parameters.AddWithValue("@numero",numero);
            return Convert.ToInt32(comando.ExecuteScalar()) > 0;
        }

    public bool Insertar(Vuelo vuelo)
    {
        using var conexion = new SqliteConnection(cadenaConexion);
        conexion.Open();

        var sql = """
                  INSERT INTO Vuelo (Id,Numero,Origen,Destino,Salida,DuracionMinutos,Filas,Precio)
                  VALUES (@id,@numero,@idOrigen,@idDestino,@salida,@duracion,@filas,@precio)
                  RETURNING Id
                  """;
        using var comando = new SqliteCommand(sql, conexion);
        comando.Parameters.AddWithValue("@id",vuelo.Id);
        comando.Parameters.AddWithValue("@numero",vuelo.Numero);
        comando.Parameters.AddWithValue("@idOrigen",vuelo.Origen);
        comando.Parameters.AddWithValue("@idDestino",vuelo.Destino);
        comando.Parameters.AddWithValue("@salida", vuelo.Salida.ToString(FormatoFecha, CultureInfo.InvariantCulture));
        comando.Parameters.AddWithValue("@duracion",vuelo.DuracionMinutos);
        comando.Parameters.AddWithValue("@filas",vuelo.Filas);
        comando.Parameters.AddWithValue("@precio",vuelo.Precio);
        return Convert.ToInt32(comando.ExecuteScalar()) > 0;
    }

    private static List<Vuelo> LeerVuelos(SqliteCommand comando)
    {
        List<Vuelo> vuelos = new List<Vuelo>();

        using var lector = comando.ExecuteReader();

        while(lector.Read())
        {
            vuelos.Add(LeerVuelo(lector,0));
        }
        return vuelos;
    }
    public static Vuelo LeerVuelo(SqliteDataReader lector, int desde) => new()
    {
        Id = lector.GetInt32(desde),
        Numero = lector.GetString(desde + 1),
        Salida = LeerFecha(lector, desde + 2),
        DuracionMinutos = lector.GetInt32(desde + 3),
        Filas = lector.GetInt32(desde + 4),
        Precio = lector.GetDecimal(desde + 5),
        Origen = new Aeropuerto { Id = lector.GetInt32(desde + 6), Codigo = lector.GetString(desde + 7), Ciudad = lector.GetString(desde + 8), Nombre = lector.GetString(desde + 9) },
        Destino = new Aeropuerto { Id = lector.GetInt32(desde + 10), Codigo = lector.GetString(desde + 11), Ciudad = lector.GetString(desde + 12), Nombre = lector.GetString(desde + 13) }
    };
    public static DateTime LeerFecha(SqliteDataReader lector, int columna) =>
        DateTime.ParseExact(lector.GetString(columna), FormatoFecha, CultureInfo.InvariantCulture);
}