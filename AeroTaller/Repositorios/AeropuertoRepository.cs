using Microsoft.Data.Sqlite;
public class AeropuertoRepository(string cadenaConexion)
{
    public List<Aeropuerto> Listar()
    {
        List<Aeropuerto> aeropuertos = new List<Aeropuerto>();

        using var conexion = new SqliteConnection(cadenaConexion);
        conexion.Open();

        var sql = "SELECT Id,Codigo,Ciudad,Nombre FROM Aeropuerto ORDER BY Ciudad";

        using var comando = new SqliteCommand(sql,conexion);
        using var lector = comando.ExecuteReader();

        while(lector.Read())
        {
            aeropuertos.Add(LeerAeropuerto(lector));
        }

        return aeropuertos;
    }

    public Aeropuerto? Obtener(int id)
    {
        using var conexion = new SqliteConnection(cadenaConexion);
        conexion.Open();

        var sql = "SELECT Id,Codigo,Ciudad,Nombre FROM Aeropuerto WHERE Id = @id";

        using var comando = new SqliteCommand(sql,conexion);
        comando.Parameters.AddWithValue("@id",id);
        using var lector = comando.ExecuteReader();
        return lector.Read() ? LeerAeropuerto(lector) : null;
    }
    private static Aeropuerto LeerAeropuerto(SqliteDataReader lector) => new()
    {
        Id = lector.GetInt32(0),
        Codigo = lector.GetString(1),
        Ciudad = lector.GetString(2),
        Nombre = lector.GetString(3)
    };

}