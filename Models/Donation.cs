using System;
using System.Collections.Generic;

namespace ApungLourdesWebApi.Models;

public partial class Donation
{
    public int DonationId { get; set; }

    public int? UserId { get; set; }

    public decimal Amount { get; set; }

    public string DonationType { get; set; } = null!;

    public string? ReferenceNo { get; set; }

    public string? Remarks { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? ModifiedAt { get; set; }

    public int? CreatedBy { get; set; }

    public int? ModifiedBy { get; set; }

    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();

    public virtual User? User { get; set; }
}
