namespace ProjektniMenadzment.Models.DTOs
{
    public class LoginRequestDto
    {
        public string EmailIliKorisnickoIme { get; set; } = string.Empty;
        public string Lozinka {  get; set; } = string.Empty;
        public bool ZapamtiMe {  get; set; }
    }
}
