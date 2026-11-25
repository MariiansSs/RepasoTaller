using Microsoft.Data.Sqlite;

public class PasajeroRepository(string cadenaConexion)
{
    public List<Pasajero> Listar()
    {
        List<Pasajero> pasajeros = new List<Pasajero>();

        using var conexion = new SqliteConnection(cadenaConexion);
        conexion.Open();

        var sql = "SELECT Id,Nombre,Dni,Email FROM Pasajero ORDER BY Nombre";

        using var comando = new SqliteCommand(sql,conexion);
        return LeerPasajeros(comando);
    } 

    public List<Pasajero> Buscar(string texto)
    {
        List<Pasajero> pasajeros = new List<Pasajero>();

        using var conexion = new SqliteConnection(cadenaConexion);
        conexion.Open();

        var sql = "SELECT Id,Nombre,Dni,Email FROM Pasajero WHERE Nombre LIKE @texto OR Dni LIKE @texto ORDER BY Nombre";

        using var comando = new SqliteCommand(sql,conexion);
        comando.Parameters.AddWithValue("@texto",$"%{texto}%");
        
        return LeerPasajeros(comando);
    }

    public Pasajero? Obtener(int id)
    {
        using var conexion = new SqliteConnection(cadenaConexion);
        conexion.Open();

        var sql = "SELECT Id,Nombre,Dni,Email FROM Pasajero WHERE Id = @id";

        using var comando = new SqliteCommand(sql,conexion);
        comando.Parameters.AddWithValue("@id",id);
        using var lector = comando.ExecuteReader();
        return lector.Read() ? LeerPasajero(lector,0) : null;
    }

    public bool Insertar(Pasajero p)
    {
        using var conexion = new SqliteConnection(cadenaConexion);
        conexion.Open();

        var sql = "INSERT INTO Pasajero(Nombre,Dni,Email) VALUES(@nombre,@dni,@email) RETURNING Id";

        using var comando = new SqliteCommand(sql,conexion);
        comando.Parameters.AddWithValue("@nombre",p.Nombre);
        comando.Parameters.AddWithValue("@dni",p.Dni);
        comando.Parameters.AddWithValue("@email", (object?)p.Email ?? DBNull.Value);

        return Convert.ToInt32(comando.ExecuteScalar()) > 0;
    }

    public bool Actualizar(Pasajero p)
    {
        using var conexion = new SqliteConnection(cadenaConexion);
        conexion.Open();

        var sql = "UPDATE Pasajero SET Nombre = @nombre, Dni = @dni, Email = @email WHERE Id = @id";

        using var comando = new SqliteCommand(sql,conexion);
        comando.Parameters.AddWithValue("@nombre",p.Nombre);
        comando.Parameters.AddWithValue("@dni",p.Dni);
        comando.Parameters.AddWithValue("@email", (object?)p.Email ?? DBNull.Value);
        comando.Parameters.AddWithValue("@id",p.Id);

        return Convert.ToInt32(comando.ExecuteScalar()) > 0;
    }

    public bool Eliminar(int id)
    {
        using var conexion = new SqliteConnection(cadenaConexion);
        conexion.Open();

        var sql = "DELETE FROM Pasajero WHERE Id = @id";

        using var comando = new SqliteCommand(sql,conexion);
        comando.Parameters.AddWithValue("@id",id);
        using var lector = comando.ExecuteReader();
        return Convert.ToInt32(comando.ExecuteScalar()) > 0;
    }

    // "excepto": al editar, el propio pasajero no cuenta como duplicado
    public bool ExisteDni(string dni, int? excepto = null)
    {
        using var conexion = new SqliteConnection(cadenaConexion);
        conexion.Open();

        var sql = "SELECT COUNT(*) Pasajero WHERE Dni = @dni";
        using var comando = new SqliteCommand(sql,conexion);
        comando.Parameters.AddWithValue("@dni",dni);
        comando.Parameters.AddWithValue("@excepto", excepto ?? 0);
        return Convert.ToInt32(comando.ExecuteScalar()) > 0;
    }

    private static Pasajero LeerPasajero(SqliteDataReader lector, int desde) => new()
    {
        Id = lector.GetInt32(0),
        Nombre = lector.GetString(1),
        Dni = lector.GetString(2),
        Email = lector.IsDBNull(3) ? null : lector.GetString(3)
    };

    private static List<Pasajero> LeerPasajeros(SqliteCommand comando)
    {
        List<Pasajero> pasajeros = new List<Pasajero>();

        using var lector = comando.ExecuteReader();

        while(lector.Read())
        {
            pasajeros.Add(LeerPasajero(lector,0));
        }
        return pasajeros;
    }
}