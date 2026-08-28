using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GymApi.DTOs
{
    public class AppUserLogin
    {
        [Required(ErrorMessage = "Email is required.")]
        [MinLength(1, ErrorMessage = "Email must be at least 1 characters long.")]
        [MaxLength(100,ErrorMessage ="Email must be at most 100 characters")]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        [Required(ErrorMessage = "Password is required.")]
        
        public string Password { get; set; } = string.Empty;
    }
}