using Microsoft.AspNetCore.Mvc;

public class HomeController(VueloRepository vuelo) : Controller
{
    // GET /  →  Views/Home/Index.cshtml
    public IActionResult Index()
    {
        // Desafío 0: colocar un punto de interrupción en la línea siguiente y recargar la página
        List<OcupacionVuelo> vuelos = vuelo.ListarOcupacion();
        return View(vuelos);
    }
}
