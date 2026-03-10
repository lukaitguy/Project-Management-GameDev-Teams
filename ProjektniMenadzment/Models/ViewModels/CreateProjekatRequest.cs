using Microsoft.AspNetCore.Mvc.Rendering;

namespace ProjektniMenadzment.Models.ViewModels
{
    public class CreateProjekatRequest
    {
        public Guid Id { get; set; }
        public string Naziv { get; set; } = null!;
        public string? Opis { get; set; }
        public string Status { get; set; } = null!;
        public decimal? Budzet { get; set; }
        public DateTime DatumPocetka { get; set; }
        public DateOnly? Rok { get; set; }
        public Guid ZanrId { get; set; }

        public List<SelectListItem>? Zanrovi { get; set; }
    }
}
