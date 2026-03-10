using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using ProjektniMenadzment.Models.Domain;

namespace ProjektniMenadzment.Models.ViewModels
{
    public class CreateResursViewModel
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Naziv resursa je obavezan.")]
        public string Naziv { get; set; } = null!;
        [Required(ErrorMessage = "Tip resursa je obavezan.")]
        public string Tip { get; set; } = null!;

        public string? Opis { get; set; }

        public decimal? Cena { get; set; }

        public Guid? ProjekatId { get; set; }
        public Guid? DodeljenKorisniku { get; set; }

        public DateTime DatumKreiranja { get; set; }

        public IEnumerable<SelectListItem>? Projekti { get; set; } 
        public IEnumerable<SelectListItem>? Korisnici { get; set; }
    }
}
