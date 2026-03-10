using System;
using System.Collections.Generic;

namespace ProjektniMenadzment.Models.Domain;

public partial class KomentariZadatak
{
    public Guid Id { get; set; }

    public string Sadrzaj { get; set; } = null!;

    public Guid ZadatakId { get; set; }

    public Guid KorisnikId { get; set; }

    public DateTime DatumKreiranja { get; set; }

    public virtual Korisnici Korisnik { get; set; } = null!;

    public virtual Zadaci Zadatak { get; set; } = null!;
}
