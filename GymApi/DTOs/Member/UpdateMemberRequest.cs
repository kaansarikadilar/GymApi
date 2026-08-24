using System.ComponentModel.DataAnnotations;
using GymApi.Models;

namespace GymApi.DTOs.Member
{
    public class UpdateMemberRequest
    {
        [Required(ErrorMessage = "MembershipTypeId is required.")]
        public Guid MembershipTypeId { get; set; }

        [Range(1, 100, ErrorMessage = "Duration must be between 1 and 100.")]
        public int DurationValue { get; set; } = 1;

        [EnumDataType(typeof(DurationUnit))]
        public DurationUnit DurationUnit { get; set; } = DurationUnit.Month;

        public string? AssignedTrainerId { get; set; }
    }
}