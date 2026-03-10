namespace ProjektniMenadzment.Models.ViewModels
{
    public class PregledProjektaViewModel
    {
        public Guid Id { get; set; }
        public string Naziv { get; set; } = null!;
        public string Opis { get; set; } = null!;
        public string Status { get; set; } = null!;
        public decimal Budzet { get; set; }
        public Guid ZanrId { get; set; }
        public string Zanr { get; set; } = null!;
        public DateTime DatumPocetka { get; set; }
        public DateOnly? Rok { get; set; }
        public string KreiraoKorisnikIme { get; set; } = null!;

        public List<ClanTimaViewModel> Clanovi { get; set; } = new();
        public List<ZadaciViewModel> Zadaci { get; set; } = new();
        public List<ResursiViewModel> Resursi { get; set; } = new();
    }
}
