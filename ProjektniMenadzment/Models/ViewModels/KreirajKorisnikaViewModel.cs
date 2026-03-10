using System.ComponentModel.DataAnnotations;

namespace ProjektniMenadzment.Models.ViewModels
{
    public class KreirajKorisnikaViewModel
    {
        [Required]
        public string UserName { get; set; } = default!;

        [Required, EmailAddress]
        public string Email { get; set; } = default!;

        [Required]
        public string Password { get; set; } = default!;

        public string[] Roles { get; set; } = Array.Empty<string>();

        public bool KreirajAppProfil { get; set; } = true;
        public string? Ime { get; set; }
        public string? Prezime { get; set; }
        public string? BrojTelefona { get; set; }
        public string? Biografija { get; set; }
    }
}
