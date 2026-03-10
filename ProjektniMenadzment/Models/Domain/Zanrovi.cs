using System;
using System.Collections.Generic;

namespace ProjektniMenadzment.Models.Domain;

public partial class Zanrovi
{
    public Guid Id { get; set; }

    public string Naziv { get; set; } = null!;

    public virtual ICollection<Projekti> Projektis { get; set; } = new List<Projekti>();
}
