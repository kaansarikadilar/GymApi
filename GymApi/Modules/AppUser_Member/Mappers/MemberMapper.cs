using GymApi.DTOs.Member;
using GymApi.Models;
using GymApi.Modules.Barcode.DTOs;

namespace GymApi.Mappers
{
    public static class MemberMapper
    {
        public static MemberResponse ToMemberResponseFromCreate(this Member member,
                                                     IEnumerable<BarcodeResponse> barcodes)
        {
            return new MemberResponse
            {
                Id = member.Id,
                FullName = member.FirstName + " " +member.LastName,
                AppUserName = member.AppUser!.FirstName + member.AppUser.LastName,
                Email = member.AppUser.Email!,
                MemberCode = member.MemberCode,
                MembershipType = member.MembershipType!.Name ?? string.Empty,
                AssignedTrainerName = member.AssignedTrainer != null 
                
                    ? $"{member.AssignedTrainer.FirstName} {member.AssignedTrainer.LastName}" 
                    : null,
                DurationValue = member.DurationValue,
                MembershipTypeCode = member.MembershipType.Code ?? string.Empty,
                DurationUnit = member.DurationUnit,
                AssignedTrainerId = member.AssignedTrainerId,
                DurationFormatted = $"{member.DurationValue} {member.DurationUnit}",
                StartDate = member.StartDate,
                EndDate = member.EndDate,
                Barcodes = barcodes ?? Enumerable.Empty<BarcodeResponse>()
            };
        }
         public static MemberResponse ToMemberResponse(this Member member)
        {
            return new MemberResponse
            {
                Id = member.Id,
                FullName = member.FirstName + " " +member.LastName,
                AppUserName = member.AppUser!.FirstName + member.AppUser.LastName,
                Email = member.AppUser.Email!,
                MemberCode = member.MemberCode,
                MembershipType = member.MembershipType!.Name ?? string.Empty,
                AssignedTrainerName = member.AssignedTrainer != null 
                
                    ? $"{member.AssignedTrainer.FirstName} {member.AssignedTrainer.LastName}" 
                    : null,
                DurationValue = member.DurationValue,
                MembershipTypeCode = member.MembershipType.Code ?? string.Empty,
                DurationUnit = member.DurationUnit,
                AssignedTrainerId = member.AssignedTrainerId,
                DurationFormatted = $"{member.DurationValue} {member.DurationUnit}",
                StartDate = member.StartDate,
                EndDate = member.EndDate
            };
        }
        public static Member ToMember(this MemberRequest memberRequest
                                      ,string generatedCode
                                      ,DateTime startDate
                                      ,DateTime endDate
                                      ,AppUser IsPresent)
        {
            return new Member
            {
                Id = Guid.NewGuid(),
                AppUserId = IsPresent.Id,
                FirstName = memberRequest.FirstName,
                LastName = memberRequest.LastName,
                MembershipTypeId = memberRequest.MembershipTypeId,
                DurationValue = memberRequest.DurationValue,
                DurationUnit = memberRequest.DurationUnit,
                AssignedTrainerId = null,
                MemberCode = generatedCode,
                StartDate = startDate,
                EndDate = endDate
            };
        }
       public static Member ToMemberFromUpdate(
    this UpdateMemberRequest memberRequest,
    Member existingMember,
    string generatedCode,
    DateTime startDate,
    DateTime endDate)
{
    existingMember.FirstName = memberRequest.FirstName;
    existingMember.LastName = memberRequest.LastName;
    existingMember.MembershipTypeId = memberRequest.MembershipTypeId;
    existingMember.DurationValue = memberRequest.DurationValue;
    existingMember.DurationUnit = memberRequest.DurationUnit;
    existingMember.MemberCode = generatedCode;
    existingMember.StartDate = startDate;
    existingMember.EndDate = endDate;

    return existingMember;
}
    }
}