using Microsoft.AspNetCore.Mvc.Rendering;

namespace ProjektniMenadzment.Models.ViewModels
{
    public class CreateZadatakRequest
    {
        public Guid Id { get; set; }
        public Guid ProjekatId { get; set; }

        public string Naslov { get; set; } = null!;
        public string? Opis { get; set; }
        public string Prioritet { get; set; } = null!;
        public DateOnly? Rok { get; set; }

        public Guid? DodeljenKorisnikuId { get; set; }
        public IEnumerable<SelectListItem> Korisnici { get; set; } = new List<SelectListItem>();
    }
}
