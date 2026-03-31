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

    public DateTime DatumKreiranja { get; set; }

    public string? VerzijaIgre { get; set; }

    public string? Engine { get; set; }

    public string? Platforma { get; set; }

    public string? FazaRazvoja { get; set; }

    public DateTime? DatumPoslednjegBuilda { get; set; }

    public virtual ICollection<Buildovi> Buildovis { get; set; } = new List<Buildovi>();

    public virtual ICollection<ClanoviProjektum> ClanoviProjekta { get; set; } = new List<ClanoviProjektum>();

    public virtual Korisnici KreiraoKorisnik { get; set; } = null!;

    public virtual ICollection<Resursi> Resursis { get; set; } = new List<Resursi>();

    public virtual ICollection<Zadaci> Zadacis { get; set; } = new List<Zadaci>();

    public virtual ICollection<Zanrovi> Zanrs { get; set; } = new List<Zanrovi>();
}
