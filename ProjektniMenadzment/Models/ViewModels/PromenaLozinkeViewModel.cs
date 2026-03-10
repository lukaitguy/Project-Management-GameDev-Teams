using System.ComponentModel.DataAnnotations;

namespace ProjektniMenadzment.Models.ViewModels
{
    public class PromenaLozinkeViewModel
    {
        public string IdentityUserId { get; set; }
        [Required]
        public string NewPassword { get; set; }
        [Required, Compare(nameof(NewPassword))]
        public string ConfirmPassword { get; set; }
    }
}
