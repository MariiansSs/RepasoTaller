using Microsoft.AspNetCore.Mvc;

public class VuelosController(VueloRepository vuelos) : Controller
{
    [HttpGet]
    public IActionResult Index()
    {
        List<Vuelo> ListVuelos = vuelos.Listar();
        return View(ListVuelos);
    }

    public IActionResult Detalle(int id)
    {
        Vuelo v = vuelos.Obtener(id);
        if(v == null)
        {
            return NotFound();
        }
        return View(v);
    }
}