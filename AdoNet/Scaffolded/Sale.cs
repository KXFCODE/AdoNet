using System;
using System.Collections.Generic;

namespace AdoNet.Scaffolded;

public partial class Sale
{
    public Guid Id { get; set; }

    /// <summary>
    /// REFERENCES Products(Id)
    /// </summary>
    public Guid ProductId { get; set; }

    /// <summary>
    /// REFERENCES Managers(Id)
    /// </summary>
    public Guid ManagerId { get; set; }

    public DateTime SaleDate { get; set; }

    public int Units { get; set; }

    public DateTime? DeleteDt { get; set; }
}
