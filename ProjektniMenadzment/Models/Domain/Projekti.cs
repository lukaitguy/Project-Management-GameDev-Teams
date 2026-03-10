using System;
using System.Collections.Generic;

namespace ProjektniMenadzment.Models.Domain;

public partial class Projekti
{
    public Guid Id { get; set; }

    public string Naziv { get; set; } = null!;

    public string? Opis { get; set; }

    public string Status { get; set; } = null!;

    public decimal? Budzet { get; set; }

    public DateTime DatumPocetka { get; set; }

    public DateOnly? Rok { get; set; }

    public Guid KreiraoKorisnikId { get; set; }

    public Guid? ZanrId { get; set; }

    public DateTime DatumKreiranja { get; set; }

    public ICollection<ClanoviProjektum> ClanoviProjekta { get; set; }
    public virtual ICollection<Resursi> Resursis { get; set; } = new List<Resursi>();

    public virtual Korisnici KreiraoKorisnik { get; set; } = null!;

    public virtual Zanrovi? Zanr { get; set; }
}
