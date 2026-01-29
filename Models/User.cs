using System;
using System.Collections.Generic;

namespace ApungLourdesWebApi.Models;

/// <summary>
///
/// </summary>
public partial class User
{
    public int UserId { get; set; }

    public string FullName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public int RoleId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? ModifiedAt { get; set; }

    public int? CreatedBy { get; set; }

    public int? ModifiedBy { get; set; }

    // ✅ ADD THESE (they exist in your DB table)
    // tinyint(1) in MySQL maps to bool in C#
    public bool IsApproved { get; set; } = false;

    // varchar(20) NOT NULL default 'Pending'
    public string Status { get; set; } = "Pending";

    // ✅ These also exist in your DB table (NOT NULL default '')
    public string CompleteAddress { get; set; } = "";

    public string PhoneNumber { get; set; } = "";

    public virtual ICollection<Donation> Donations { get; set; } = new List<Donation>();

    public virtual Role Role { get; set; } = null!;
}
