﻿using System.ComponentModel.DataAnnotations;

namespace PracticeCRUDApi.Dto
{
    public class RegisterDto
    {
        [Required]
        [StringLength(50)]
        public required string Username { get; set; }

        [Required]
        [EmailAddress]
        public required string Email { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 6)]
        public required string Password { get; set; }
    }

    public class LoginDto
    {
        [Required]
        [StringLength(50)]
        public required string Username { get; set; }
        [Required]
        [StringLength(100, MinimumLength = 6)]
        public required string Password { get; set; }
    }

    public class UpdateUserDto
    {
        public string Email { get; set; } = string.Empty;
        public string? NewPassword { get; set; }  // 可選填
    }
}
