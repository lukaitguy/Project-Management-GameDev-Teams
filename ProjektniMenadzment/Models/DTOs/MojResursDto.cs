namespace ProjektniMenadzment.Models.DTOs
{
    public class MojResursDto
    {
        public Guid Id { get; set; }
        public Guid ProjekatId { get; set; }
        public string ProjekatNaziv { get; set; } = null!;
        public string Naziv { get; set; } = null!;
        public string Tip { get; set; } = null!;
        public string? Opis { get; set; }
        public decimal? Cena { get; set; }
        public Guid? DodeljenKorisniku { get; set; }
        public string? DodeljenKorisnikuIme { get; set; }
        public DateTime DatumKreiranja { get; set; }
    }
}
