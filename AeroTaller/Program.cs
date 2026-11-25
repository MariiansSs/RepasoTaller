using System.Globalization;
using Microsoft.Data.Sqlite;

// AeroTaller — proyecto base del TP integrador.
// Se completa desafío por desafío siguiendo la Torre de Control (dotnet run --project torre-de-control).

// Fechas y moneda en castellano, en cualquier computadora.
// Decimales: un <input type="number"> SIEMPRE envía "18.5" (con punto), y con es-AR sin modificar
// el model binding lo leería como 185. Por eso los números (no la moneda) usan punto decimal.
var cultura = (CultureInfo)CultureInfo.GetCultureInfo("es-AR").Clone();
cultura.NumberFormat.NumberDecimalSeparator = ".";
cultura.NumberFormat.NumberGroupSeparator = "";
CultureInfo.DefaultThreadCurrentCulture = cultura;

var builder = WebApplication.CreateBuilder(args);

// Desafío 1: leer la cadena de conexión y crear la base si no existe
string cadenaConexion = builder.Configuration.GetConnectionString("AeroTaller")
    ?? throw new InvalidOperationException("Falta la cadena de conexion 'AeroTaller' en appsettings.json");
CrearBaseSiNoExiste(cadenaConexion);
// Desafío 3 y 7: registrar repositorios (y después servicios) en el contenedor
builder.Services.AddScoped(_ => new AeropuertoRepository(cadenaConexion));
builder.Services.AddScoped(_ => new VueloRepository(cadenaConexion));
builder.Services.AddScoped(_ => new PasajeroRepository(cadenaConexion));
builder.Services.AddScoped(_ => new ReservaRepository(cadenaConexion));
// Desafío 8: Serilog
// Desafío 9: sesión

// MVC: controladores que devuelven vistas
builder.Services.AddControllersWithViews();

var app = builder.Build();

app.UseStaticFiles();   // sirve lo que está en wwwroot (el CSS)

// Ruteo convencional: /Vuelos/Detalle/3 → VuelosController.Detalle(3)
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

// Si la base no existe, la crea con aerotaller.sql
static void CrearBaseSiNoExiste(string cadenaConexion)
{
    // De la cadena se toma solo el archivo: "aerotaller.db" (o la ruta que indique el verificador)
    var archivo = new SqliteConnectionStringBuilder(cadenaConexion).DataSource;
    if (File.Exists(archivo)) return;

    using var conexion = new SqliteConnection(cadenaConexion);
    conexion.Open();                       // SQLite crea el archivo vacío al abrir

    using var comando = conexion.CreateCommand();
    // El script está junto al ejecutable (bin/…), no en la carpeta del proyecto
    comando.CommandText = File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "aerotaller.sql"));
    comando.ExecuteNonQuery();             // ejecuta TODO el script: DROP, CREATE e INSERT
}


// dotnet add package Microsoft.Data.Sqlite
// dotnet new mvc -o ClinicaMvc
/* {
  "ConnectionStrings": {
    "AeroTaller": "Data Source=aerotaller.db;Foreign Keys=True"
  },
  "Logging": { … lo que ya estaba … }
}
*/
