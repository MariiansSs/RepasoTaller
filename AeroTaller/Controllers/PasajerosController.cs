using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;

public class PasajerosController(PasajeroRepository pasajeros, ReservaRepository reservas) : Controller
{
    public IActionResult Index(string? buscar)
    {
        List<Pasajero> lista = string.IsNullOrWhiteSpace(buscar)
            ? pasajeros.Listar()
            : pasajeros.Buscar(buscar);

        ViewData["Buscar"] = buscar;  
        return View(lista);
    }

    public IActionResult Detalle(int id)
    {
        Pasajero? pasajero = pasajeros.Obtener(id);
        if (pasajero == null)
        {
            return NotFound();
        } 
        var modelo = new PasajeroDetalleViewModel(pasajero, reservas.ListarPorPasajero(id));
        return View(modelo);            

    }
}