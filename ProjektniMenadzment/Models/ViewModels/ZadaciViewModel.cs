using ProjektniMenadzment.Models.Domain;

namespace ProjektniMenadzment.Models.ViewModels
{
    public class ZadaciViewModel
    {
        public Guid Id { get; set; }

        public Guid ProjekatId { get; set; }

        public string Naslov { get; set; } = null!;

        public string? Opis { get; set; }

        public string Status { get; set; } = null!;

        public string Prioritet { get; set; } = null!;

        public string? DodeljenKorisnikuIme { get; set; }

        public DateOnly? Rok { get; set; }

        public DateTime DatumKreiranja { get; set; }
        public string Komentar { get; set; }
        public IEnumerable<ZadaciKomentariViewModel> Komentari { get; set; }

    }
}
