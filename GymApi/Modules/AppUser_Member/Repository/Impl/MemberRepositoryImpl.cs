using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GymApi.Data;
using GymApi.DTOs.Member;
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
            _context.Members.Remove(user);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Member>> GetAllAsync()
        {
            return await _context.Members
            .Include(m=>m.AppUser)
            .Include(m=>m.MembershipType)
            .Include(m=>m.AssignedTrainer)
            .AsAsyncEnumerable()
            .ToListAsync();
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
            .Include(m=>m.AppUser)
            .Include(m=>m.MembershipType)
            .Include(m=>m.AssignedTrainer)
            .FirstOrDefaultAsync(m=>m.AppUser!.Email == email);
        }
    }
}