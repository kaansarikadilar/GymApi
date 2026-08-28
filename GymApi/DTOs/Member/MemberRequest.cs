using System.ComponentModel.DataAnnotations;
using GymApi.Models;

namespace GymApi.DTOs.Member
{
    public class MemberRequest
    {
        [Required(ErrorMessage = "First Name is required.")]
        [MinLength(1, ErrorMessage = "First Name must be at least 1 characters long.")]
        [MaxLength(50,ErrorMessage ="First Name must be at most 50 characters")]
        public string FirstName { get; set; } = string.Empty;
        [Required(ErrorMessage = "Last name is required.")]
        [MinLength(1, ErrorMessage = "Last Name must be at least 1 characters long.")]
        [MaxLength(50,ErrorMessage ="Last Name must be at most 50 characters")]
        public string LastName { get; set; } = string.Empty;
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "MembershipTypeId is required.")]
        public int MembershipTypeId { get; set; }

        [Range(1, 730, ErrorMessage = "Duration must be between 1 and 730.")]
        public int DurationValue { get; set; } = 1;

        [EnumDataType(typeof(DurationUnit))]
        public DurationUnit DurationUnit { get; set; } = DurationUnit.Month;
        public string? AssignedTrainerId { get; set; }
    }
}