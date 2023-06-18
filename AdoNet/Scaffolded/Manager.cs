using System;
using System.Collections.Generic;

namespace AdoNet.Scaffolded;

public partial class Manager
{
    public Guid Id { get; set; }

    public string Surname { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string Secname { get; set; } = null!;

    public Guid IdMainDep { get; set; }

    public Guid? IdSecDep { get; set; }

    public Guid? IdChief { get; set; }

    public DateTime? FiredDt { get; set; }
}
