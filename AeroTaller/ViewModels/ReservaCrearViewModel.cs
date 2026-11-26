using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

public class ReservaCrearViewModel
{
    // Lo que carga el usuario (vuelve en el POST)
    [Display(Name = "Vuelo")]
    public int? VueloId { get; set; }

    [Display(Name = "Pasajero")]
    public int? PasajeroId { get; set; }

    [Display(Name = "Asiento")]
    public string Asiento { get; set; } = "";

    // Datos que usa la vista para dibujarse (NO vuelven en el POST)
    public List<SelectListItem> Vuelos { get; set; } = [];
    public List<SelectListItem> Pasajeros { get; set; } = [];
}