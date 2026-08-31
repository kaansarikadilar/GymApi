using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GymApi.DTOs
{
    public class AppUserRequest
    {
        [Required(ErrorMessage = "First name is required.")]
        [MinLength(1, ErrorMessage = "First Name must be at least 1 characters long.")]
        [MaxLength(50,ErrorMessage ="Fist Name must be at most 50 characters")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Last name is required.")]
        [MinLength(1, ErrorMessage = "Last Name must be at least 1 characters long.")]
        [MaxLength(50,ErrorMessage ="Last Name must be at most 50 characters")]

        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email address is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        [MinLength(1, ErrorMessage = "Email must be at least 1 characters long.")]
        [MaxLength(100,ErrorMessage ="Email must be at most 100 characters")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required.")]
        public string Password { get; set; } = string.Empty;

        // Role to assign upon registration ("User", "Trainer", "Admin")[cite: 1]
         [MinLength(1, ErrorMessage = "Role must be at least 1 characters long.")]
        [MaxLength(10,ErrorMessage ="Role must be at most 10 characters")]
        public string Role { get; set; } = "User";
    }
}