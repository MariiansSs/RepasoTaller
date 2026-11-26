public class PasajeroDetalleViewModel(Pasajero pasajero, List<Reserva> reservas)
{
    public int Id { get; } = pasajero.Id;
    public string Nombre { get; } = pasajero.Nombre;
    public string Dni { get; } = pasajero.Dni;
    public string Email { get; } = pasajero.Email ?? "—";
    public List<ReservaListadoViewModel> Reservas { get; } =
        reservas.Select(r => new ReservaListadoViewModel(r)).ToList();
}