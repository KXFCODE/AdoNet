using System;
using System.Collections.Generic;

namespace AdoNet.Scaffolded;

public partial class Department
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public DateTime? DeleteDt { get; set; }
}
