using System;
using System.Collections.Generic;

namespace ApungLourdesWebApi.Models;

public partial class Transaction
{
    public int TransactionId { get; set; }

    public int? DonationId { get; set; }

    public string PaymentMethod { get; set; } = null!;

    public string ReferenceNo { get; set; } = null!;

    public string? Status { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? ModifiedAt { get; set; }

    public int? CreatedBy { get; set; }

    public int? ModifiedBy { get; set; }

    public virtual Donation? Donation { get; set; }
}
