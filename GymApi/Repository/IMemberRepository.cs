using GymApi.Models;

namespace GymApi.Repository
{
    public interface IMemberRepository
    {
        Task<Member> CreateAsync(Member member);
        Task<Member?> GetByIdAsync(Guid id);
        Task<Member?> GetMemberByEmail(string email);
        Task<Member?> GetByAppUserIdAsync(string appUserId);
        Task<IEnumerable<Member>> GetAllAsync();
        Task<Member> UpdateAsync(Member member);
        Task<bool> DeleteAsync(string email);
    }
}