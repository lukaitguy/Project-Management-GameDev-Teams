using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ProjektniMenadzment.Models.ViewModels
{
    public class AddMemberViewModel
    {
        public Guid ProjekatId { get; set; }
        [Required(ErrorMessage = "Korisnik je obavezan")]
        public Guid KorisnikId { get; set; }
        [Required(ErrorMessage = "Uloga je obavezna")]
        public string Uloga { get; set; }

        public List<SelectListItem> Korisnici { get; set; } = new();
    }
}
