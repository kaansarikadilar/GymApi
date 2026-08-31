using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GymApi.DTOs.Member;
using Refit;

namespace GymApi.Modules.Barcode.Clients
{
    public interface IMemberApiClient
    {
        [Get("/api/members/{id}")]
        Task<MemberResponse?> GetMemberByIdAsync(Guid id);
    }
}