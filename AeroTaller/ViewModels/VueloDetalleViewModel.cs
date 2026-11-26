// ViewModels/VueloDetalleViewModel.cs
public class VueloDetalleViewModel
{
    public int Id { get; }
    public string Numero { get; }
    public string Ruta { get; }
    public string Salida { get; }
    public string Llegada { get; }
    public string Precio { get; }
    public int Capacidad { get; }
    public int Ocupados { get; }
    public bool PuedeReservar { get; }

    // Cada elemento es una FILA del avión; cada fila tiene 6 asientos (A–F)
    public List<List<AsientoViewModel>> Mapa { get; } = [];

    public VueloDetalleViewModel(Vuelo vuelo, List<string> asientosOcupados)
    {
        Id = vuelo.Id;
        Numero = vuelo.Numero;
        Ruta = $"{vuelo.Origen} → {vuelo.Destino}";        // usa el ToString() de Aeropuerto
        Salida = vuelo.Salida.ToString("dddd dd/MM/yyyy HH:mm");
        Llegada = vuelo.Llegada.ToString("dddd dd/MM/yyyy HH:mm");
        Precio = vuelo.Precio.ToString("C0");
        Capacidad = vuelo.Capacidad;
        Ocupados = asientosOcupados.Count;
        PuedeReservar = !vuelo.YaSalio && Ocupados < Capacidad;

        var ocupados = asientosOcupados.ToHashSet();   // HashSet: "¿está?" sin recorrer toda la lista
        for (int fila = 1; fila <= vuelo.Filas; fila++)
        {
            var asientos = new List<AsientoViewModel>();
            foreach (char letra in Vuelo.LetrasAsiento)
            {
                string codigo = $"{fila}{letra}";
                asientos.Add(new AsientoViewModel(codigo, ocupados.Contains(codigo)));
            }
            Mapa.Add(asientos);
        }
    }
}