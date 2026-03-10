using ProjektniMenadzment.Models.Domain;

namespace ProjektniMenadzment.Models.ViewModels
{
    public class KorisniciViewModel
    {
        public string IdentityUserId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string[] Roles { get; set; } = Array.Empty<string>();

        public Guid? Id { get; set; }
        public string? Ime { get; set; }
        public string? Prezime { get; set; }
        public string? BrojTelefona { get; set; }
        public string? Biografija { get; set; }
        
        public int ProjektiCount { get; set; }
        public int ZadaciCount { get; set; }

        public bool isLinked => Id.HasValue;

    }
}
