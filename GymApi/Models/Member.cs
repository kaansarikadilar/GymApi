using System.ComponentModel.DataAnnotations;

namespace GymApi.Models
{
    public class Member
    {
        public Guid Id { get; set; }

        // Links to the Identity User account
        public string AppUserId { get; set; } = string.Empty;
        public AppUser? AppUser { get; set; }
        public string MemberCode { get; set; } = string.Empty;

        // Foreign key linking to MembershipType (ST, VP, OG) enums
        public int MembershipTypeId { get; set; }
        public MembershipTypes? MembershipType { get; set; }

        // Duration quantity (e.g., 1, 3, 12)
        public int DurationValue { get; set; } 

        // Unit type (Day, Month, Year)
        public DurationUnit DurationUnit { get; set; }

        // Optional link to an assigned Personal Trainer
        public string? AssignedTrainerId { get; set; }
        public AppUser? AssignedTrainer { get; set; }

        public DateTime StartDate { get; set; } = DateTime.UtcNow;
        public DateTime EndDate { get; set; }
    }
}