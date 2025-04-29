using System;
using System.Collections.Generic;

namespace MobileCare.Models;

public partial class Customer
{
    public int CustomerId { get; set; }

    public string FullName { get; set; } = null!;

    public string PhoneNumber { get; set; } = null!;

    public virtual ICollection<Repairorder> Repairorders { get; set; } = new List<Repairorder>();
}
