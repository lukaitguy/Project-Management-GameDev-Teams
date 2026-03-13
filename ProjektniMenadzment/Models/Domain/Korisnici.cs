using System;
using System.Collections.Generic;

namespace ProjektniMenadzment.Models.Domain;

public partial class Korisnici
{
    public Guid Id { get; set; }

    public string Ime { get; set; } = null!;

    public string Prezime { get; set; } = null!;

    public string? BrojTelefona { get; set; }

    public string Email { get; set; } = null!;

    public string? Biografija { get; set; }

    public DateTime DatumKreiranja { get; set; }

    public string? IdentityUserId { get; set; }

    public virtual ICollection<ClanoviProjektum> ClanoviProjekta { get; set; } = new List<ClanoviProjektum>();

    public virtual ICollection<KomentariZadatak> KomentariZadataks { get; set; } = new List<KomentariZadatak>();

    public virtual ICollection<Resursi> Resursis { get; set; } = new List<Resursi>();

    public virtual ICollection<Zadaci> ZadaciDodeljenKorisnikus { get; set; } = new List<Zadaci>();

    public virtual ICollection<Zadaci> ZadaciKreiraoKorisniks { get; set; } = new List<Zadaci>();
}
