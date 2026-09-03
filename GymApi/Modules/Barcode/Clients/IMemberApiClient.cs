using System;
using System.Threading.Tasks;
using GymApi.DTOs.Member;
using Refit;

namespace GymApi.Modules.Barcode.Clients
{
    public interface IMemberApiClient
    {
        [Get("/api/members/{id}")]
        Task<MemberResponse> GetMemberByIdAsync(Guid id, [Header("Authorization")] string token);

        [Get("/api/members/by-email")]
        Task<MemberResponse?> GetMemberByEmail([Query] string email, [Header("Authorization")] string token);
    }
}