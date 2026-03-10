namespace ProjektniMenadzment.Models.DTOs
{
    public class RegisterRequestDto
    {
        public string Ime { get; set; } = string.Empty;
        public string Prezime { get; set; } = string.Empty;
        public string KorisnickoIme { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? BrojTelefona {  get; set; }
        public string Lozinka { get; set; } = string.Empty;
    }
}
