namespace ProjektniMenadzment.Models.DTOs
{
    public class ClanProjekatDto
    {
        public Guid KorisnikId { get; set; }
        public string Ime { get; set; } = null!;
        public string Prezime { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? Uloga { get; set; }
    }
}
