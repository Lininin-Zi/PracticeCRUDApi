using System;
using System.Collections.Generic;

namespace PracticeCRUDApi.Models;

public partial class User
{
    public int Id { get; set; }

    public string Username { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public string PasswordSalt { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    /*
    

    
    public string? PhoneNumber { get; set; }

    public string? Address { get; set; }

    

    public DateTime? LastLoginAt { get; set; }

    public bool IsActive { get; set; }

    public virtual ICollection<LoginHistory> LoginHistories { get; set; } = new List<LoginHistory>();

    public virtual ICollection<PasswordResetToken> PasswordResetTokens { get; set; } = new List<PasswordResetToken>();

    public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    */
}
