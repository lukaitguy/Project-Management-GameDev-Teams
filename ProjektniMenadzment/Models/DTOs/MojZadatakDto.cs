namespace ProjektniMenadzment.Models.DTOs
{
    public class MojZadatakDto
    {
        public Guid Id { get; set; }
        public Guid ProjekatId { get; set; }
        public string ProjekatNaziv { get; set; } = null!;
        public string Naslov { get; set; } = null!;
        public string? Opis { get; set; }
        public string Status { get; set; } = null!;
        public string Prioritet { get; set; } = null!;
        public string? TipZadatka { get; set; }
        public DateOnly? Rok { get; set; }
        public DateTime DatumKreiranja { get; set; }
        public Guid? DodeljenKorisnikuId { get; set; }
        public string? DodeljenKorisnikuIme { get; set; }
    }
}
