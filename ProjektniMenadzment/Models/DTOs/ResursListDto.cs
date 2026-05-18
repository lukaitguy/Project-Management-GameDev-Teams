namespace ProjektniMenadzment.Models.DTOs
{
    public class ResursListDto
    {
        public Guid Id { get; set; }
        public string Naziv { get; set; } = null!;
        public string Tip { get; set; } = null!;
        public string? Opis { get; set; }
        public decimal? Cena { get; set; }
        public Guid? DodeljenKorisniku { get; set; }
        public string? DodeljenKorisnikuIme { get; set; }
        public DateTime DatumKreiranja { get; set; }
    }
}
