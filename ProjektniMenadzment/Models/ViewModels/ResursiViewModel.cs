namespace ProjektniMenadzment.Models.ViewModels
{
    public class ResursiViewModel
    {
        public Guid Id { get; set; }

        public string Naziv { get; set; } = null!;

        public string Tip { get; set; } = null!;

        public string? Opis { get; set; }

        public decimal? Cena { get; set; }

        public string? Projekat { get; set; }

        public string? DodeljenKorisniku { get; set; }

        public DateTime DatumKreiranja { get; set; }
    }
}
