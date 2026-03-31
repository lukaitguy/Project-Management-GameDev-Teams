namespace ProjektniMenadzment.Models.DTOs
{
    public class UpdateProjekatDto
    {
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

        public List<Guid> ZanrIds { get; set; } = new();
        public bool DodajNoviZanr { get; set; }
        public string? NoviZanrNaziv { get; set; }
    }
}
