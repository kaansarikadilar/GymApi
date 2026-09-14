using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GymApi.Data;
using GymApi.DTOs.Member;
using GymApi.Helpers;
using GymApi.Models;
using GymApi.Service;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace GymApi.Repository.Impl
{
    public class MemberRepositoryImpl : IMemberRepository
    {
        private readonly ApplicationDbContext _context;

        public MemberRepositoryImpl(
            ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<Member> CreateAsync(Member member)
        {
            await _context.Members.AddAsync(member);
            await _context.SaveChangesAsync();
            return member;
        }
         public async Task<Member> UpdateAsync(Member member)
        {
            _context.Members.Update(member);
            await _context.SaveChangesAsync();
            return member;
        }
        public async Task<bool> DeleteAsync(string email)
        {
           var user = await _context.Members.FirstOrDefaultAsync(m=>m.AppUser!.Email == email);
           if(user == null)
            {
                return false;
            }
            
            var TrainedUser = _context.Members.Where(a=>a.AssignedTrainerId == user.Id);
            foreach(var trainee in TrainedUser)
            {
                trainee.AssignedTrainerId = null;
            }

            _context.Members.Remove(user);
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<IEnumerable<Member>> GetAllAsync(MemberQueryObject queryObject)
        {
            var members = _context.Members
            .Include(m=>m.AppUser)
            .Include(m=>m.MembershipType)
            .Include(m=>m.AssignedTrainer)
            .ThenInclude(t=>t!.AppUser)
            .AsNoTracking()
            .AsQueryable();

            if (queryObject.Id.HasValue)
            {
                members = _context.Members.Where(a=>a.Id == queryObject.Id.Value);
            }
            if (!string.IsNullOrWhiteSpace(queryObject.Email))
            {
                members = members.Where(m => m.AppUser != null && m.AppUser.Email!.ToLower().Contains(queryObject.Email.ToLower()));
            }
            if (queryObject.DoesTrainer.HasValue)
            {
                members = queryObject.DoesTrainer.Value 
                ? members.Where(a=>a.AssignedTrainer != null)
                : members.Where(a=>a.AssignedTrainer == null);
            }
            if (!string.IsNullOrWhiteSpace(queryObject.SortBy))
            {
                if (queryObject.SortBy.Equals("StartDate", StringComparison.OrdinalIgnoreCase))
                {
                    members = queryObject.IsDescending 
                        ? members.OrderByDescending(m => m.StartDate) 
                        : members.OrderBy(m => m.StartDate);
                }
                else if (queryObject.SortBy.Equals("EndDate", StringComparison.OrdinalIgnoreCase))
                {
                    members = queryObject.IsDescending 
                        ? members.OrderByDescending(m => m.EndDate) 
                        : members.OrderBy(m => m.EndDate);
                }
                else if (queryObject.SortBy.Equals("FirstName", StringComparison.OrdinalIgnoreCase))
                {
                    members = queryObject.IsDescending 
                        ? members.OrderByDescending(m => m.FirstName) 
                        : members.OrderBy(m => m.FirstName);
                }
            }
            else
            {
                members = members.OrderByDescending(m => m.StartDate);
            }
            var skipNumber = (queryObject.PageNumber -1 ) * queryObject.PageSize;

            return await members.Skip(skipNumber).Take(queryObject.PageSize).ToListAsync();
        }
        public async Task<Member?> GetByAppUserIdAsync(string appUserId)
        {
        return await _context.Members
            .Include(m=>m.AppUser)
            .Include(m=>m.MembershipType)
            .Include(m=>m.AssignedTrainer)
            .FirstOrDefaultAsync(m=>m.AppUserId == appUserId);
        }

        public async Task<Member?> GetByIdAsync(Guid id)
        {
            return await _context.Members
            .Include(m=>m.AppUser)
            .Include(m=>m.MembershipType)
            .Include(m=>m.AssignedTrainer)
            .FirstOrDefaultAsync(m=>m.Id == id);
        }

        public async Task<Member?> GetMemberByEmail(string email)
        {
         return await _context.Members
        .Include(m => m.AppUser)
        .Include(m => m.MembershipType)
        .Include(m => m.AssignedTrainer)
        .FirstOrDefaultAsync(m => m.AppUser!.Email == email);
        }
    }
}