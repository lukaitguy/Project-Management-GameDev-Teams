using System;
using System.Collections.Generic;

namespace ProjektniMenadzment.Models.Domain;

public partial class Resursi
{
    public Guid Id { get; set; }

    public string Naziv { get; set; } = null!;

    public string Tip { get; set; } = null!;

    public string? Opis { get; set; }

    public decimal? Cena { get; set; }

    public Guid? ProjekatId { get; set; }

    public Guid? DodeljenKorisniku { get; set; }

    public DateTime DatumKreiranja { get; set; }

    public virtual Korisnici? DodeljenKorisnikuNavigation { get; set; }

    public virtual Projekti? Projekat { get; set; }
}
