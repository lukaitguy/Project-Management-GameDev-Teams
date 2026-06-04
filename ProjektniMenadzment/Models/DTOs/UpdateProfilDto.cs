namespace ProjektniMenadzment.Models.DTOs
{
    public class UpdateProfilDto
    {
        public string Ime { get; set; } = null!;
        public string Prezime { get; set; } = null!;
        public string? BrojTelefona { get; set; }
        public string? Biografija { get; set; }
    }
}
