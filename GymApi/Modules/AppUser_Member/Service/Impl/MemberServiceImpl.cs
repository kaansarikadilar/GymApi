using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using GymApi.Data;
using GymApi.DTOs.Member;
using GymApi.Mappers;
using GymApi.Models;
using GymApi.Modules.Barcode.Clients;
using GymApi.Modules.Barcode.DTOs;
using GymApi.Repository;
using Microsoft.EntityFrameworkCore;

namespace GymApi.Service.Impl
{
    public class MemberServiceImpl : IMemberService
    {
        private readonly IMemberRepository _memberRepo;
        private readonly UserManager<AppUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly IBarcodeApiClient _barcodeApi;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public MemberServiceImpl(
            IMemberRepository memberRepo,
            ApplicationDbContext context,
            UserManager<AppUser> userManager,
            IBarcodeApiClient barcodeApi,
            IHttpContextAccessor httpContextAccessor)
        {
            _memberRepo = memberRepo;
            _context = context;
            _userManager = userManager;
            _barcodeApi = barcodeApi;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<MemberResponse?> CreateMemberAsync(MemberRequest request)
        {
            // 1. Guard against duplicate member account creation
            var existingMember = await _memberRepo.GetMemberByEmail(request.Email);
            if (existingMember != null)
            {
                return null;
            }

            var isPresent = await _userManager.FindByEmailAsync(request.Email);
            if (isPresent == null)
            {
                return null;
            }

            var type = await _context.MembershipTypes.FindAsync(request.MembershipTypeId);
            if (type == null)
            {
                return null;
            } 

            string sequenceNumber = Random.Shared.Next(100, 999).ToString();
            string generatedCode = $"{type.Code}{sequenceNumber}";
            DateTime startDate = DateTime.UtcNow;
            DateTime endDate = request.DurationUnit switch
            {
                DurationUnit.Day => startDate.AddDays(request.DurationValue),
                DurationUnit.Month => startDate.AddMonths(request.DurationValue),
                DurationUnit.Year => startDate.AddYears(request.DurationValue),
                _ => startDate.AddMonths(request.DurationValue)
            };

            var user = request.ToMember(generatedCode, startDate, endDate, isPresent);
            user.AppUser = isPresent;
            user.MembershipType = type;

            if (request.AssignedTrainerId.HasValue)
            {
                var trainerMember = await _context.Members
                .Include(m=>m.AppUser).
                FirstOrDefaultAsync(m=>m.Id == request.AssignedTrainerId);
                if(trainerMember?.AppUser == null || !await _userManager.IsInRoleAsync(trainerMember.AppUser, "Trainer"))
                {
                    return null;
                }
            }

            await _memberRepo.CreateAsync(user);

            var token = GetAuthorizationToken();
            var barcodeResponse = await _barcodeApi.BarcodeGeneration(isPresent.Email!, token);

            return user.ToMemberResponse(barcodeResponse ?? Enumerable.Empty<BarcodeResponse>());
        }

        public async Task<MemberResponse?> UpdateMemberAsync(string email, UpdateMemberRequest request)
        {
            var token = GetAuthorizationToken();
            var existingUser = await _userManager.FindByEmailAsync(email);
            var databaseUser = await _memberRepo.GetMemberByEmail(email);
            if (existingUser == null || databaseUser == null)
            {
                return null;
            }
            var type = await _context.MembershipTypes.FindAsync(request.MembershipTypeId);
            if (type == null)
            {
                return null;
            }

            string generatedCode = databaseUser.MemberCode;
            if (databaseUser.MembershipTypeId != request.MembershipTypeId || string.IsNullOrWhiteSpace(generatedCode))
            {
                string sequenceNumber = Random.Shared.Next(100, 999).ToString();
                generatedCode = $"{type.Code}{sequenceNumber}";
            }

            DateTime startDate = databaseUser.StartDate;
            DateTime endDate = request.DurationUnit switch
            {
                DurationUnit.Day => startDate.AddDays(request.DurationValue),
                DurationUnit.Month => startDate.AddMonths(request.DurationValue),
                DurationUnit.Year => startDate.AddYears(request.DurationValue),
                _ => startDate.AddMonths(request.DurationValue)
            };
            

            var user = request.ToMemberFromUpdate(databaseUser, generatedCode, startDate, endDate);
            user.AppUser = existingUser;
            user.MembershipType = type;

            if (request.AssignedTrainerId.HasValue)
            {
                var trainerMember = await _context.Members
                .Include(m=>m.AppUser).
                FirstOrDefaultAsync(m=>m.Id == request.AssignedTrainerId);
                if(trainerMember?.AppUser == null || !await _userManager.IsInRoleAsync(trainerMember.AppUser, "Trainer"))
                {
                    return null;
                }
            }

            await _memberRepo.UpdateAsync(user);

            var barcodes = await _barcodeApi.BarcodeGeneration(email, token);
            return user.ToMemberResponse(barcodes ?? Enumerable.Empty<BarcodeResponse>());
        }

        public async Task<bool> DeleteMemberAsync(string email)
        {
            var appendedUser = _context.Members.Where(a=>a.AssignedTrainer !=null);
            var user = await _memberRepo.DeleteAsync(email);
            return user;
        }

        public async Task<IEnumerable<MemberResponse>> GetAllMembersAsync()
        {
            var allUsers = await _memberRepo.GetAllAsync();
            if (!allUsers.Any())
            {
                return Enumerable.Empty<MemberResponse>();
            }

            var token = GetAuthorizationToken();
            var barcodes = (await _barcodeApi.GetAllBarcodes(token)) ?? Enumerable.Empty<BarcodeResponse>();

            var responses = new List<MemberResponse>();
            foreach (var member in allUsers)
            {
                // Filter only the barcodes that belong to this specific member
                var memberBarcodes = barcodes.Where(b => 
                    string.Equals(b.Email, member.AppUser?.Email, StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(b.MemberCode, member.MemberCode, StringComparison.OrdinalIgnoreCase));

                responses.Add(member.ToMemberResponse(memberBarcodes));
            }

            return responses;
        }

        public async Task<MemberResponse?> GetByIdAsync(Guid id)
        {
            var user = await _memberRepo.GetByIdAsync(id);
            if (user == null)
            {
                return null;
            }

            var token = GetAuthorizationToken();
            var barcodes = await _barcodeApi.GetBarcodeByMemberId(id, token);

            // Never return null if barcodes are empty; return member with an empty barcode collection
            return user.ToMemberResponse(barcodes ?? Enumerable.Empty<BarcodeResponse>());
        }

        public async Task<MemberResponse?> GetMemberByEmail(string email)
        {
            var user = await _memberRepo.GetMemberByEmail(email);
            if (user == null)
            {
                return null;
            }

            var token = GetAuthorizationToken();
            var barcodes = await _barcodeApi.GetBarcodeByMemberEmail(email, token);

            // Never return null if barcodes are empty; return member with an empty barcode collection
            return user.ToMemberResponse(barcodes ?? Enumerable.Empty<BarcodeResponse>());
        }

        private string GetAuthorizationToken()
        {
            return _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString() ?? string.Empty;
        }
    }
}