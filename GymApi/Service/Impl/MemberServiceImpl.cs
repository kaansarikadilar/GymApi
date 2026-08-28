using GymApi.Data;
using GymApi.DTOs.Member;
using GymApi.Models;
using GymApi.Repository;
using Microsoft.AspNetCore.Identity;
using GymApi.Mappers;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Runtime.CompilerServices;

namespace GymApi.Service.Impl
{
    public class MemberServiceImpl : IMemberService
    {
        private readonly IMemberRepository _memberRepo;
        private readonly UserManager<AppUser> _userManager;
        private readonly ApplicationDbContext _context;
        public MemberServiceImpl(
            IMemberRepository memberRepo,
            ApplicationDbContext context,
            UserManager<AppUser> userManager)
        {
            _memberRepo = memberRepo;
            _context = context;
            _userManager = userManager;
        }
        public async Task<MemberResponse?> CreateMemberAsync(MemberRequest request)
        {
            var IsPresent = await _userManager.FindByEmailAsync(request.Email);
            if(IsPresent == null)
            {
                return null;
            }
            var type = await _context.MembershipTypes.FindAsync(request.MembershipTypeId);
            if(type == null)
            {
                return null;
            }
            string uniqueSuffix = Guid.NewGuid().ToString("N").Substring(0, 6).ToUpper();
            string generatedCode = $"{type.Code}-{uniqueSuffix}";   
            DateTime startDate = DateTime.UtcNow;
            DateTime endDate = request.DurationUnit switch
        {
            DurationUnit.Day => startDate.AddDays(request.DurationValue),
            DurationUnit.Month => startDate.AddMonths(request.DurationValue),
            DurationUnit.Year => startDate.AddYears(request.DurationValue),
            _ => startDate.AddMonths(request.DurationValue)
        };
        var user = request.ToMember(generatedCode,startDate,endDate,IsPresent);
        
        await _memberRepo.CreateAsync(user);

        return user.ToMemberResponse();
        }
        public async Task<MemberResponse?> UpdateMemberAsync(string email, UpdateMemberRequest request)
        {
            var ExistingUser = await _userManager.FindByEmailAsync(email);
            var DatabaseUser = await _memberRepo.GetMemberByEmail(email);
            if(ExistingUser == null)
            {
                return null;
            }
            var type = await _context.MembershipTypes.FindAsync(request.MembershipTypeId);
            if(type == null)
            {
                return null;
            }
            string uniqueSuffix = Guid.NewGuid().ToString("N").Substring(0, 6).ToUpper();
            string generatedCode = $"{type.Code}-{uniqueSuffix}";   
            DateTime startDate = DateTime.UtcNow;
            DateTime endDate = request.DurationUnit switch
        {
            DurationUnit.Day => startDate.AddDays(request.DurationValue),
            DurationUnit.Month => startDate.AddMonths(request.DurationValue),
            DurationUnit.Year => startDate.AddYears(request.DurationValue),
            _ => startDate.AddMonths(request.DurationValue)
        };
           var user = request.ToMemberFromUpdate(DatabaseUser!, generatedCode, startDate, endDate);
          await _memberRepo.UpdateAsync(user);
          return user.ToMemberResponse();
        }
        public async Task<bool> DeleteMemberAsync(string email)
        {
            var user = await _memberRepo.DeleteAsync(email);
            if(user == false)
            {
                return false;
            }
            return true;
        }
        public async Task<IEnumerable<MemberResponse>> GetAllMembersAsync()
        {
            var AllUsers = await _memberRepo.GetAllAsync();
            return AllUsers.Select(m=>m.ToMemberResponse());
        }

        public async Task<MemberResponse?> GetByIdAsync(Guid id)
        {
            var user = await _memberRepo.GetByIdAsync(id);
            if(user == null)
            {
                return null;
            }
            return user.ToMemberResponse();
        }
        public async Task<MemberResponse?> GetMemberByEmail(string email)
        {
            var user = await _memberRepo.GetMemberByEmail(email);
                if(user == null)
            {
                return null;
            }
            return user.ToMemberResponse();
        }
    }
}