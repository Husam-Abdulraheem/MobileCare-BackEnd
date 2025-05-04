using System;
using System.Collections.Generic;

namespace MobileCare.Models;

public partial class Repairorder
{
    public int RepairOrderId { get; set; }

    public int CustomerId { get; set; }

    public int DeviceId { get; set; }

    public int UserId { get; set; }

    public decimal? EstimatedCost { get; set; }

    public string? Status { get; set; }

    public string? ProblemDescription { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Customer Customer { get; set; } = null!;

    public virtual Device Device { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
