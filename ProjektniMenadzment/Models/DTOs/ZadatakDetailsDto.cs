namespace ProjektniMenadzment.Models.DTOs
{
    public class ZadatakDetailsDto
    {
        public Guid Id { get; set; }
        public Guid ProjekatId { get; set; }
        public string Naslov { get; set; } = null!;
        public string? Opis { get; set; }
        public string Status { get; set; } = null!;
        public string Prioritet { get; set; } = null!;
        public string? TipZadatka { get; set; }
        public DateOnly? Rok { get; set; }
        public DateTime DatumKreiranja { get; set; }
        public Guid KreiraoKorisnikId { get; set; }
        public string KreiraoKorisnikIme { get; set; } = null!;
        public Guid? DodeljenKorisnikuId { get; set; }
        public string? DodeljenKorisnikuIme { get; set; }
    }
}
