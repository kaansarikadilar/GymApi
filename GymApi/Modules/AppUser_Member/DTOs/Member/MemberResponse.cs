using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GymApi.DTOs.Member
{
    public class MemberResponse
    {
        public Guid Id { get; set; }
        public string AppUserName { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string MemberCode { get; set; } = string.Empty;
        public string MembershipType { get; set; } = string.Empty;
        public string? AssignedTrainerName { get; set; } 
        public string? AssignedTrainerId { get; set; }
        public string DurationFormatted { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}