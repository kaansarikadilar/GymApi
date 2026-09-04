using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GymApi.Models;
using GymApi.Modules.Barcode.DTOs;

namespace GymApi.DTOs.Member
{
    public class MemberResponse
    {
        public Guid Id { get; set; }
        public string AppUserName { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string MemberCode { get; set; } = string.Empty; // assigned code
        public string MembershipType { get; set; } = string.Empty; //VIP,STANDART,OGRENCI
        public string MembershipTypeCode { get; set; } = string.Empty; //VP,ST,OG,KR
        public string? AssignedTrainerName { get; set; } 
        public Guid? AssignedTrainerId { get; set; }
        public int DurationValue { get; set; }
        public DurationUnit DurationUnit { get; set; }
        public string DurationFormatted { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public IEnumerable<BarcodeResponse> Barcodes { get; set; } = Enumerable.Empty<BarcodeResponse>();    }
}