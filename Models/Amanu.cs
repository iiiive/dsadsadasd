using System;
using System.Collections.Generic;

namespace ApungLourdesWebApi.Models;

public partial class Amanu
{
    public int Id { get; set; }

    public string Type { get; set; } = null!;

    public string Title { get; set; } = null!;

    public DateOnly Date { get; set; }

    public string? Theme { get; set; }

    public string? Scripture { get; set; }

    public string? Reading { get; set; }

    public string Content { get; set; } = null!;

    public string CreatedBy { get; set; } = null!;

    public string? ModifiedBy { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime ModifiedAt { get; set; }
}
