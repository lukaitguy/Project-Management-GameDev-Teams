namespace ProjektniMenadzment.Models.DTOs
{
    public class ProfilDto
    {
        public string KorisnickoIme { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Ime { get; set; } = null!;
        public string Prezime { get; set; } = null!;
        public string? BrojTelefona { get; set; }
        public string? Biografija { get; set; }
        public DateTime DatumKreiranja { get; set; }
    }
}
