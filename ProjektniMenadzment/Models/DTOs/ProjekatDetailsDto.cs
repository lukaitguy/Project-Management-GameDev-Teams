namespace ProjektniMenadzment.Models.DTOs
{
    public class ProjekatDetailsDto
    {
        public Guid Id { get; set; }

        public string Naziv { get; set; } = string.Empty;

        public string? Opis { get; set; }

        public string Status { get; set; } = string.Empty;

        public decimal? Budzet { get; set; }

        public DateTime DatumPocetka { get; set; }

        public DateOnly? Rok { get; set; }

        public string? VerzijaIgre { get; set; }

        public string? Engine { get; set; }

        public string? Platforma { get; set; }

        public string? FazaRazvoja { get; set; }

        public DateTime? DatumPoslednjegBuilda { get; set; }
    }
}
