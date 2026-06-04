namespace ProjektniMenadzment.Models.DTOs
{
    public class ProjekatIzvestajDto
    {
        public Guid Id { get; set; }
        public string Naziv { get; set; } = null!;
        public string Status { get; set; } = null!;
        public string? FazaRazvoja { get; set; }
        public string? Platforma { get; set; }
        public string? Engine { get; set; }
        public DateTime DatumPocetka { get; set; }
        public DateOnly? Rok { get; set; }
        public decimal? Budzet { get; set; }

        // Task statistics
        public int UkupnoZadataka { get; set; }
        public int ZavrsenihZadataka { get; set; }
        public int UTokuZadataka { get; set; }
        public int NijeZapocetihZadataka { get; set; }
        public int PauziranihZadataka { get; set; }
        public int OtkazanihZadataka { get; set; }
        public int ZakasnelihZadataka { get; set; }
        public double PostotakZavrsenosti { get; set; }

        // Team analysis
        public List<ClanIzvestajDto> Clanovi { get; set; } = new();

        // Finance
        public decimal UkupniTroskoviResursa { get; set; }
        public decimal? PreostaliButzet { get; set; }
        public List<ResursIzvestajDto> Resursi { get; set; } = new();
    }

    public class ClanIzvestajDto
    {
        public string ImeIPrezime { get; set; } = null!;
        public string? Uloga { get; set; }
        public int UkupnoZadataka { get; set; }
        public int ZavrsenihZadataka { get; set; }
        public int AktivnihZadataka { get; set; }
    }

    public class ResursIzvestajDto
    {
        public string Naziv { get; set; } = null!;
        public string Tip { get; set; } = null!;
        public decimal? Cena { get; set; }
        public string? DodeljenKorisnikuIme { get; set; }
    }
}
