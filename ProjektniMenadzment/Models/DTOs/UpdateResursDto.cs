namespace ProjektniMenadzment.Models.DTOs
{
    public class UpdateResursDto
    {
        public string Naziv { get; set; } = null!;
        public string Tip { get; set; } = null!;
        public string? Opis { get; set; }
        public decimal? Cena { get; set; }
        public Guid? DodeljenKorisniku { get; set; }
    }
}
