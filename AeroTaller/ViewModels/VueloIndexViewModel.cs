using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;   // SelectListItem

public class VueloIndexViewModel
{
    // Lo que viene en el query string: /Vuelos?DestinoId=2&Desde=2026-12-01
    [Display(Name = "Destino")]
    public int? DestinoId { get; set; }

    [Display(Name = "Desde")]
    [DataType(DataType.Date)]                 // asp-for genera <input type="date">
    public DateTime? Desde { get; set; }

    // Datos que usa la vista para dibujarse
    public List<SelectListItem> Destinos { get; set; } = [];
    public List<VueloListadoViewModel> Resultados { get; set; } = [];
}