using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;

public class PasajerosController(PasajeroRepository pasajeros, ReservaRepository reservas) : Controller
{
    public IActionResult Index(string? buscar)
    {
        List<Pasajero> lista = string.IsNullOrWhiteSpace(buscar)
            ? pasajeros.Listar()
            : pasajeros.Buscar(buscar);

        ViewData["Buscar"] = buscar;    // para que el buscador muestre lo que se buscó
        return View(lista);
    }

    public IActionResult Detalle(int id)
    {
        Pasajero p = pasajeros.Obtener(id);
        if(p == null)
        {
            return NotFound();
        }

        ViewData["Reservas"] = reservas.ListarPorPasajero(id);

        return View(p);
        
    }
}