namespace ProjektniMenadzment.Models.DTOs
{
    public class CreateZadatakDto
    {
        public string Naslov { get; set; } = null!;
        public string? Opis { get; set; }
        public string Status { get; set; } = null!;
        public string Prioritet { get; set; } = null!;
        public string? TipZadatka { get; set; }
        public DateOnly? Rok { get; set; }
        public Guid? DodeljenKorisnikuId { get; set; }
    }
}
