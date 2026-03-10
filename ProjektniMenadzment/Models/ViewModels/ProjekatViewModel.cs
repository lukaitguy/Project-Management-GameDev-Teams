namespace ProjektniMenadzment.Models.ViewModels
{
    public class ProjekatViewModel
    {
        public Guid Id { get; set; }
        public string Naziv { get; set; } = null!;
        public string Status { get; set; } = null!;
        public DateTime DatumPocetka { get; set; }
        public DateOnly? Rok { get; set; }
        public string KreiraoKorisnikIme { get; set; } = null!;
    }
}
