using System.Globalization; // permite CultureInfo
using Microsoft.Data.Sqlite;

public class ReservaRepository(string cadenaConexion)
{
    public List<Reserva> Listar(EstadoReserva? estado)
    {
        using var conexion = new SqliteConnection(cadenaConexion);
        conexion.Open();

        var sql = """
                  SELECT r.Id,r.Asiento,r.Fecha,r.Estado
                         v.Id,v.Numero,v.Salida,v.DuracionMinutos,v.Filas,v.Precio,
                         o.Id, o.Codigo, o.Ciudad, o.Nombre,
                         d.Id, d.Codigo, d.Ciudad, d.Nombre,
                         p.Id,p.Nombre,p.Dni,p.Email
                  FROM Reserva r
                  INNER JOIN Vuelo v      ON r.VueloId    = v.Id
                  INNER JOIN Aeropuerto o ON v.OrigenId   = o.Id
                  INNER JOIN Aeropuerto d ON v.DestinoId  = d.Id
                  INNER JOIN Pasajero p   ON r.PasajeroId = p.Id
                  WHERE Estado = @estado
                  """;
        using var comando = new SqliteCommand(sql, conexion);
        return LeerReservas(comando);
    }

    public List<Reserva> ListarPorPasajero(int pasajeroId)
    {
        using var conexion = new SqliteConnection(cadenaConexion);
        conexion.Open();

        var sql = """
                  SELECT r.Id,r.Asiento,r.Fecha,r.Estado,
                         v.Id,v.Numero,v.Salida,v.DuracionMinutos,v.Filas,v.Precio,
                         o.Id,o.Codigo,o.Ciudad,o.Nombre,
                         d.Id,d.Codigo,d.Ciudad,d.Nombre,
                         p.Id,p.Nombre,p.Dni,p.Email
                  FROM Reserva r
                  INNER JOIN Vuelo v      ON r.VueloId    = v.Id
                  INNER JOIN Aeropuerto o ON v.OrigenId   = o.Id
                  INNER JOIN Aeropuerto d ON v.DestinoId  = d.Id
                  INNER JOIN Pasajero p   ON r.PasajeroId = p.Id
                  WHERE p.Id = @pasajeroId
                  """;
        using var comando = new SqliteCommand(sql,conexion);
        comando.Parameters.AddWithValue("@pasajeroId",pasajeroId);
        return LeerReservas(comando);
    }

    public Reserva? Obtener(int id)
    {
        using var conexion = new SqliteConnection(cadenaConexion);
        conexion.Open();

        var sql = "SELECT Id,Asiento,Fecha,Estado FROM Reserva WHERE Id = @id";
        using var comando = new SqliteCommand(sql,conexion);
        comando.Parameters.AddWithValue("@id",id);
        using var lector = comando.ExecuteReader();
        return LeerReserva(lector,0);
    }

    public List<string> AsientosOcupados(int vueloId)
    {
        List<string> asientos = new List<string>();

        using var conexion = new SqliteConnection(cadenaConexion);
        conexion.Open();

        var sql = "SELECT Asiento FROM Reserva WHERE IdVuelo = @idVuelo AND Estado = confirmado";
        using var comando = new SqliteCommand(sql,conexion);
        comando.Parameters.AddWithValue("@idVuelo",vueloId);
        using var lector = comando.ExecuteReader();

        while(lector.Read())
        {
            asientos.Add(lector.GetString(0));
        }

        return asientos;
    }

    public int ContarConfirmadas(int vueloId)
    {
        using var conexion = new SqliteConnection(cadenaConexion);
        conexion.Open();

        var sql = "SELECT COUNT(*) FROM Reserva WHERE IdVuelo = @idVuelo AND Estado = confirmado";
        using var comando = new SqliteCommand(sql,conexion);
        comando.Parameters.AddWithValue("@idVuelo",vueloId);
        using var lector = comando.ExecuteReader();
        return lector.Read() ? lector.GetInt32(0) : 0;
    }

    public bool PasajeroTieneReserva(int vueloId, int pasajeroId)
    {
        using var conexion = new SqliteConnection(cadenaConexion);
        conexion.Open();

        var sql = """
                  SELECT COUNT(*) FROM Reserva
                  WHERE r.estado = confirmado AND r.VueloId = @vueloId AND p.Id = @pasajeroId
                  """;
        using var comando = new SqliteCommand(sql,conexion);
        comando.Parameters.AddWithValue("@idVuelo",vueloId);
        comando.Parameters.AddWithValue("pasajeroId",pasajeroId);
        using var lector = comando.ExecuteReader();
        return Convert.ToInt32(comando.ExecuteScalar()) > 0;
    }

    public bool Insertar(Reserva reserva)
    {
        using var conexion = new SqliteConnection(cadenaConexion);
        conexion.Open();

        var sql =   """
                    INSERT INTO Reserva (vueloId,pasajeroId,Asiento,Fecha,Estado)
                    VALUES (@vueloId,@pasajeroId,@asiento,@fecha,@estado)
                    RETURNING Id
                    """;
        using var comando = new SqliteCommand(sql,conexion);
        comando.Parameters.AddWithValue("@vueloId",reserva.Vuelo.Id);
        comando.Parameters.AddWithValue("@pasajeroId",reserva.Pasajero.Id);
        comando.Parameters.AddWithValue("@asiento",reserva.Asiento);
         comando.Parameters.AddWithValue("@fecha", reserva.Fecha.ToString(VueloRepository.FormatoFecha, CultureInfo.InvariantCulture));
        comando.Parameters.AddWithValue("@estado", reserva.Estado.ToString());

        return Convert.ToInt32(comando.ExecuteScalar()) > 0;
    }

    public bool CambiarEstado(int id, EstadoReserva estado)
    {
        using var conexion = new SqliteConnection(cadenaConexion);
        conexion.Open();

        var sql = "UPDATE Reserva SET estado = @estado WHERE Id = @id";
        using var comando = new SqliteCommand(sql,conexion);
        comando.Parameters.AddWithValue("@id",id);
        comando.Parameters.AddWithValue("@estado", estado);
        return Convert.ToInt32(comando.ExecuteScalar()) > 0;
    }
    private static List<Reserva> LeerReservas(SqliteCommand comando)
    {
        var reservas = new List<Reserva>();

        using var lector = comando.ExecuteReader();
        while (lector.Read())
        {
            reservas.Add(new Reserva
            {
                Id = lector.GetInt32(0),
                Asiento = lector.GetString(1),
                Fecha = VueloRepository.LeerFecha(lector, 2),
                Estado = Enum.Parse<EstadoReserva>(lector.GetString(3)),   // texto → enum
                Vuelo = VueloRepository.LeerVuelo(lector, 4),
                Pasajero = PasajeroRepository.LeerPasajero(lector, 18)
            });
        }
        return reservas;
    }

    private static Reserva LeerReserva(SqliteDataReader lector, int desde) => new()
    {
        Id = lector.GetInt32(0),
        Asiento = lector.GetString(1),
        Fecha = lector.GetDateTime(2),
        Estado = Enum.Parse<EstadoReserva>(lector.GetString(3))
    };
}