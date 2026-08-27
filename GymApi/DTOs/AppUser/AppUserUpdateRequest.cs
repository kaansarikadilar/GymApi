using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GymApi.DTOs.AppUser
{
    public class AppUserUpdateRequest
    {
        [Required(ErrorMessage = "First name is required.")]
        [MinLength(1, ErrorMessage = "First Name must be at least 1 characters long.")]
        [MaxLength(50,ErrorMessage ="Fist Name must be at most 50 characters")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Last name is required.")]
        [MinLength(1, ErrorMessage = "Last Name must be at least 1 characters long.")]
        [MaxLength(50,ErrorMessage ="Last Name must be at most 50 characters")]

        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password address is required.")]
        [MinLength(1, ErrorMessage = "Password must be at least 1 characters long.")]
        [MaxLength(100,ErrorMessage ="Password must be at most 100 characters")]
        public string ExistingPassword { get; set; } = string.Empty;
        [MinLength(1, ErrorMessage = "Password must be at least 1 characters long.")]
        [MaxLength(100,ErrorMessage ="Password must be at most 100 characters")]
        public string? NewPassword { get; set; } 
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        [MinLength(1, ErrorMessage = "Password must be at least 1 characters long.")]
        [MaxLength(100,ErrorMessage ="Password must be at most 100 characters")]
        public string? NewEmail { get; set; }

        // Role to assign upon registration ("User", "Trainer", "Admin")[cite: 1]
        [MinLength(1, ErrorMessage = "Role must be at least 1 characters long.")]
        [MaxLength(10,ErrorMessage ="Role must be at most 10 characters")]
        [Required]
        public string Role { get; set; } = "User";
    }
}