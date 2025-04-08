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
}
