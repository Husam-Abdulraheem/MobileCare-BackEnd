using System;
using System.Collections.Generic;

namespace MobileCare.Models;

public partial class Device
{
    public int DeviceId { get; set; }

    public string Brand { get; set; } = null!;

    public string Model { get; set; } = null!;

    public string? Imei { get; set; }

    public string? DeviceCondition { get; set; }

    public virtual ICollection<Repairorder> Repairorders { get; set; } = new List<Repairorder>();
}
