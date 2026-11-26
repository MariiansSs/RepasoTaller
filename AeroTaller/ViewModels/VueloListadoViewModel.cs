public class VueloListadoViewModel(Vuelo vuelo, int ocupados)
{
    public int Id { get; } = vuelo.Id;
    public string Numero { get; } = vuelo.Numero;
    public string Ruta { get; } = $"{vuelo.Origen.Codigo} → {vuelo.Destino.Codigo}";
    public string Ciudades { get; } = $"{vuelo.Origen.Ciudad} → {vuelo.Destino.Ciudad}";
    public string Salida { get; } = vuelo.Salida.ToString("ddd dd/MM HH:mm");          // "lun 23/11 07:30"
    public string Llegada { get; } = vuelo.Llegada.ToString("HH:mm") + (vuelo.LlegaAlOtroDia ? " (+1)" : "");
    public string Duracion { get; } = $"{vuelo.DuracionMinutos / 60} h {vuelo.DuracionMinutos % 60:00} min";
    public string Precio { get; } = vuelo.Precio.ToString("C0");                       // "$ 98.000"
    public int Libres { get; } = vuelo.Capacidad - ocupados;
    public bool YaSalio { get; } = vuelo.YaSalio;
}