using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GymApi.DTOs.Member;
using GymApi.Helpers;

namespace GymApi.Service
{
    public interface IMemberService
    {
        Task<MemberResponse?> CreateMemberAsync(MemberRequest request);
        Task<MemberResponse?> GetMemberByEmail(string email);
        Task<MemberResponse?> GetByIdAsync(Guid id);
        Task<IEnumerable<MemberResponse>> GetAllMembersAsync(MemberQueryObject queryObject);
        Task<MemberResponse?> UpdateMemberAsync(string mail, UpdateMemberRequest request);
        Task<bool> DeleteMemberAsync(string email);
        Task<bool> DeleteMemberBarcodeAsync(string email);
    }
}