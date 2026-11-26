public class ReservaListadoViewModel(Reserva reserva)
{
    public int Id { get; } = reserva.Id;
    public int VueloId { get; } = reserva.Vuelo.Id;
    public string Vuelo { get; } = reserva.Vuelo.Numero;
    public string Ruta { get; } = $"{reserva.Vuelo.Origen.Codigo} → {reserva.Vuelo.Destino.Codigo}";
    public string Salida { get; } = reserva.Vuelo.Salida.ToString("dd/MM HH:mm");
    public int PasajeroId { get; } = reserva.Pasajero.Id;
    public string Pasajero { get; } = reserva.Pasajero.Nombre;
    public string Asiento { get; } = reserva.Asiento;
    public string Estado { get; } = reserva.Estado.ToString();       // enum → texto
    public bool Cancelada { get; } = reserva.Estado == EstadoReserva.Cancelada;
}

public class ReservaIndexViewModel
{
    public EstadoReserva? Estado { get; set; }
    public List<ReservaListadoViewModel> Reservas { get; set; } = [];
}