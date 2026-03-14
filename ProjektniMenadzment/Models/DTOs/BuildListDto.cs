namespace ProjektniMenadzment.Models.DTOs
{
    public class BuildListDto
    {
        public Guid Id { get; set; }
        public string Verzija { get; set; } = string.Empty;
        public string? NazivBuilda { get; set; }
        public string? TipBuilda { get; set; }
        public string? PatchNapomene { get; set; }
        public DateTime? DatumBuilda { get; set; }
        public Guid ProjekatId { get; set; }

    }
}
