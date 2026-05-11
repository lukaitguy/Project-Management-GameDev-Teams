namespace ProjektniMenadzment.Models.DTOs
{
    public class KomentarDto
    {
        public Guid Id { get; set; }
        public string Sadrzaj { get; set; } = null!;
        public Guid ZadatakId { get; set; }
        public Guid KorisnikId { get; set; }
        public string KorisnikIme { get; set; } = null!;
        public DateTime DatumKreiranja { get; set; }
    }
}
