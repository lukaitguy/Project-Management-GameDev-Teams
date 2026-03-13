using System;
using System.Collections.Generic;

namespace ProjektniMenadzment.Models.Domain;

public partial class Buildovi
{
    public Guid Id { get; set; }

    public Guid ProjekatId { get; set; }

    public string Verzija { get; set; } = null!;

    public string? NazivBuilda { get; set; }

    public string? TipBuilda { get; set; }

    public string? PatchNapomene { get; set; }

    public DateTime? DatumBuilda { get; set; }

    public virtual Projekti Projekat { get; set; } = null!;
}
