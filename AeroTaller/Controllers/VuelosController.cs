using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering; // SelectListItem
public class VuelosController(
    VueloRepository vuelos,
    AeropuertoRepository aeropuertos,
    ReservaRepository reservas) : Controller
{
    public IActionResult Index(VueloIndexViewModel filtro)
    {
        List<Vuelo> encontrados = vuelos.Buscar(filtro.DestinoId, filtro.Desde);

        // Una sola consulta con GROUP BY para todos (no una consulta por vuelo)
        Dictionary<int, int> ocupados = vuelos.ListarOcupacion()
            .ToDictionary(o => o.VueloId, o => o.Ocupados);

        filtro.Resultados = encontrados
            .Select(v => new VueloListadoViewModel(v, ocupados.GetValueOrDefault(v.Id)))
            .ToList();
        filtro.Destinos = aeropuertos.Listar()
            .Select(a => new SelectListItem($"{a.Ciudad} ({a.Codigo})", a.Id.ToString()))
            .ToList();
        return View(filtro);
    }

    public IActionResult Detalle(int id)
    {
        Vuelo? vuelo = vuelos.Obtener(id);
        if (vuelo == null)
        {
            return NotFound();  
        }        

        var modelo = new VueloDetalleViewModel(vuelo, reservas.AsientosOcupados(id));
        return View(modelo);
    }

    
}