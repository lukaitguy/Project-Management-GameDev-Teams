namespace ProjektniMenadzment.Models.Domain
{
    public class ClanoviProjektum
    {
        public Guid ProjekatId { get; set; }
        public Guid KorisnikId { get; set; }
        public string Uloga { get; set; } = null!;

        public virtual Projekti Projekat { get; set; } = null!;
        public virtual Korisnici Korisnik { get; set; } = null!;
    }
}
