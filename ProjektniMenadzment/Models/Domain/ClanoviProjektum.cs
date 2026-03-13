using System;
using System.Collections.Generic;

namespace ProjektniMenadzment.Models.Domain;

public partial class ClanoviProjektum
{
    public Guid ProjekatId { get; set; }

    public Guid KorisnikId { get; set; }

    public string? Uloga { get; set; }

    public virtual Korisnici Korisnik { get; set; } = null!;

    public virtual Projekti Projekat { get; set; } = null!;
}
