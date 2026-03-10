using System;
using System.Collections.Generic;

namespace ProjektniMenadzment.Models.Domain;

public partial class Zadaci
{
    public Guid Id { get; set; }

    public Guid ProjekatId { get; set; }

    public string Naslov { get; set; } = null!;

    public string? Opis { get; set; }

    public string Status { get; set; } = null!;

    public string Prioritet { get; set; } = null!;

    public Guid KreiraoKorisnikId { get; set; }

    public Guid? DodeljenKorisnikuId { get; set; }

    public DateOnly? Rok { get; set; }

    public DateTime DatumKreiranja { get; set; }

    public virtual Korisnici? DodeljenKorisniku { get; set; }

    public virtual ICollection<KomentariZadatak> KomentariZadataks { get; set; } = new List<KomentariZadatak>();

    public virtual Korisnici KreiraoKorisnik { get; set; } = null!;
}
