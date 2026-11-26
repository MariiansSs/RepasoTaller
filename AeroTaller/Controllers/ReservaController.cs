using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

public class ReservasController(
    ReservaRepository reservas,
    VueloRepository vuelos,
    PasajeroRepository pasajeros) : Controller
{
    // GET /Reservas  ·  GET /Reservas?estado=Confirmada
    public IActionResult Index(EstadoReserva? estado)
    {
        var modelo = new ReservaIndexViewModel
        {
            Estado = estado,
            Reservas = reservas.Listar(estado).Select(r => new ReservaListadoViewModel(r)).ToList()
        };
        return View(modelo);
    }
    [HttpGet]
    public IActionResult Crear(int? vueloId, string? asiento)
    {
        var modelo = new ReservaCrearViewModel { VueloId = vueloId, Asiento = asiento ?? "" };
        CargarListas(modelo);
        return View(modelo);
    }

    private void CargarListas(ReservaCrearViewModel modelo)
    {
        modelo.Vuelos = vuelos.Listar()
            .Where(v => !v.YaSalio)
            .Select(v => new SelectListItem($"{v.Numero} · {v.Origen.Codigo} → {v.Destino.Codigo} · {v.Salida:dd/MM HH:mm}", v.Id.ToString()))
            .ToList();
        modelo.Pasajeros = pasajeros.Listar()
            .Select(p => new SelectListItem($"{p.Nombre} ({p.Dni})", p.Id.ToString()))
            .ToList();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Crear(ReservaCrearViewModel modelo)
    {
        Vuelo? vuelo = modelo.VueloId is null ? null : vuelos.Obtener(modelo.VueloId.Value);
        Pasajero? pasajero = modelo.PasajeroId is null ? null : pasajeros.Obtener(modelo.PasajeroId.Value);

        if (vuelo is null || pasajero is null || string.IsNullOrWhiteSpace(modelo.Asiento))
        {
            CargarListas(modelo);      
            return View(modelo);
        }

        var reserva = new Reserva
        {
            Vuelo = vuelo,
            Pasajero = pasajero,
            Asiento = modelo.Asiento.Trim().ToUpperInvariant(),   // "5c " → "5C"
            Fecha = DateTime.Now,
            Estado = EstadoReserva.Confirmada
        };
        reservas.Insertar(reserva);

        // POST → Redirect → GET: se vuelve al mapa y el asiento reservado aparece ocupado
        return RedirectToAction("Detalle", "Vuelos", new { id = vuelo.Id });
    }
}