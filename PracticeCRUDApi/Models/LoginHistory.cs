using System;
using System.Collections.Generic;

namespace PracticeCRUDApi.Models;

public partial class LoginHistory
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public DateTime LoginTime { get; set; }

    public string? IpAddress { get; set; }

    public string? UserAgent { get; set; }

    public bool IsSuccessful { get; set; }

    public string? FailureReason { get; set; }

    public virtual User User { get; set; } = null!;
}
