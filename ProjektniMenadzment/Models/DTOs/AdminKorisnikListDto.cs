namespace ProjektniMenadzment.Models.DTOs
{
    public class AdminKorisnikListDto
    {
        public string IdentityUserId { get; set; } = null!;
        public string UserName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string[] Roles { get; set; } = Array.Empty<string>();
        public string? Ime { get; set; }
        public string? Prezime { get; set; }
        public string? BrojTelefona { get; set; }
        public string? Biografija { get; set; }
        public int ProjektiCount { get; set; }
        public int ZadaciCount { get; set; }
    }
}
