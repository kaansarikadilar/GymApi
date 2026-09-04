using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using GymApi.DTOs.Member;
using GymApi.Models;
using GymApi.Modules.Barcode.Clients;
using GymApi.Modules.Barcode.DTOs;
using GymApi.Modules.Barcode.Mappers;
using GymApi.Modules.Barcode.Models;
using GymApi.Modules.Barcode.Repository;

namespace GymApi.Modules.Barcode.Service.BarcodeServiceImpl
{
    public class BarcodeServiceImpl : IBarcodeService
    {
        private readonly IBarcodeRepository _barcodeRepo;
        private readonly IMemberApiClient _memberApi;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public BarcodeServiceImpl(
            IBarcodeRepository barcodeRepository,
            IMemberApiClient memberApi,
            IHttpContextAccessor httpContext)
        {
            _barcodeRepo = barcodeRepository;
            _memberApi = memberApi;
            _httpContextAccessor = httpContext;
        }
        public async Task<IEnumerable<BarcodeResponse?>> BarcodeGeneration(string email)
        {
            var token = GetAuthorizationToken();
            var user = await _memberApi.GetMemberByEmail(email,token);
            if(user == null)
            {
                return Enumerable.Empty<BarcodeResponse>();
            }

            // Check existing active barcodes
            var existingBarcodes = (await _barcodeRepo.GetActiveBarcodesByMemberIdAsync(user.Id)).ToList();
            var ruleEntities = DetermineBarcodesForMember(user);

            var entitiesToSave = ruleEntities
            .Where(e => !existingBarcodes.Any(existing => existing.Types == e.Types))
            .ToList();

            if (!entitiesToSave.Any())
            {
                return existingBarcodes.ToResponseList();
            }
            var savedEntities = new List<BarcodeEntity>();

            foreach (var entity in entitiesToSave)
            {
                var saved = await _barcodeRepo.AddBarcodeAsync(entity);
                savedEntities.Add(saved);
            }

            return existingBarcodes.Concat(savedEntities).ToResponseList();  
        }
        public async Task<IEnumerable<BarcodeResponse>> ManualBarcodeCreation(BarcodeRequest request)
        {
            var token = GetAuthorizationToken();
            var user = await _memberApi.GetMemberByEmail(request.Email,token);
            if(user == null)
            {
                return Enumerable.Empty<BarcodeResponse>();
            }
            var existingBarcodes = (await _barcodeRepo.GetActiveBarcodesByMemberIdAsync(user.Id)).ToList();
            var allowedBarcodes = DetermineBarcodesForMember(user);

            if (request.BarcodeType.HasValue)
            {
                var targetType = request.BarcodeType.Value;

                // 1. If user already has this specific barcode, return it
                var existing = existingBarcodes.FirstOrDefault(b => b.Types == targetType);
                if (existing != null)
                {
                    return new List<BarcodeResponse> { existing.ToBarcodeResponse() };
                }

                // 2. Find if this requested barcode type is valid for the user's membership
                var entityToSave = allowedBarcodes.FirstOrDefault(b => b.Types == targetType);
                if (entityToSave == null)
                {
                    // The member's package does not allow this barcode type
                    return Enumerable.Empty<BarcodeResponse>();
                }

                // 3. Save and return ONLY the specifically requested barcode
                var saved = await _barcodeRepo.AddBarcodeAsync(entityToSave);
                return new List<BarcodeResponse> { saved.ToBarcodeResponse() };
    
            }
            var entitiesToSave = allowedBarcodes
                .Where(e => !existingBarcodes.Any(existing => existing.Types == e.Types))
                .ToList();

            if (!entitiesToSave.Any())
            {
                return existingBarcodes.ToResponseList();
            }

            var savedEntities = new List<BarcodeEntity>();
            foreach (var entity in entitiesToSave)
            {
                var saved = await _barcodeRepo.AddBarcodeAsync(entity);
                savedEntities.Add(saved);
            }

            var allActiveBarcodes = existingBarcodes.Concat(savedEntities);
            return allActiveBarcodes.ToResponseList();
        }
        private string GetAuthorizationToken()
        {
            return _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString() ?? string.Empty;
        }

        private List<BarcodeEntity> DetermineBarcodesForMember(MemberResponse user)
        {
            var entities = new List<BarcodeEntity>();
            string Normalize(string input) =>
                (input ?? string.Empty)
                    .Trim()
                    .ToUpperInvariant()
                    .Replace("ı", "I")
                    .Replace("Ö", "O").Replace("Ğ", "G").Replace("Ü", "U")
                    .Replace("Ş", "S").Replace("İ", "I").Replace("Ç", "C");

            var typeCode = Normalize(user.MembershipTypeCode);
            var typeName = Normalize(user.MembershipType);

            bool isVip      = typeCode == "VP" || typeName.Contains("VIP");
            bool isStandard = typeCode == "ST" || typeName.Contains("STANDART") || typeName.Contains("STANDARD");
            bool isOgrenci  = typeCode == "OG" || typeName.Contains("OGRENCI");
            bool isKurumsal = typeCode == "KR" || typeName.Contains("KURUMSAL");

            if (isVip)
            {
                if (user.DurationUnit == DurationUnit.Year)
                {
                    entities.Add(BuildGymEntrance(user));
                    entities.Add(BuildSpaSaunaBarcode(user));
                    entities.Add(BuildPrivateLessonBarcode(user, remainingSession: 20));
                }
                else if (user.DurationUnit == DurationUnit.Month)
                {
                    entities.Add(BuildGymEntrance(user));
                    entities.Add(BuildSpaSaunaBarcode(user));
                    entities.Add(BuildPrivateLessonBarcode(user, remainingSession: 5));
                }
                else if(user.DurationUnit == DurationUnit.Day)
                {
                    entities.Add(BuildGymEntrance(user));
                }
            }
            else if (isStandard)
            {
                if (user.DurationUnit == DurationUnit.Year) // yearly standart
                {
                    entities.Add(BuildGymEntrance(user));
                    entities.Add(BuildGroupLessonBarcode(user));
                    entities.Add(BuildPrivateLessonBarcode(user, remainingSession: 5));
                }
                else if (user.DurationUnit == DurationUnit.Month) // monthly standart
                {
                    entities.Add(BuildGymEntrance(user));
                    entities.Add(BuildGroupLessonBarcode(user));
                }
                else if(user.DurationUnit == DurationUnit.Day)
                {
                    entities.Add(BuildGymEntrance(user));
                }
            }
            else if (isOgrenci)
            {
                if(user.DurationUnit == DurationUnit.Year)
                {
                    entities.Add(BuildGymEntrance(user));
                    entities.Add(BuildPrivateLessonBarcode(user, remainingSession: 6));
                }
                else if(user.DurationUnit == DurationUnit.Month)
                {
                    entities.Add(BuildGymEntrance(user));
                    entities.Add(BuildPrivateLessonBarcode(user, remainingSession: 1));
                }
                else if(user.DurationUnit == DurationUnit.Day)
                {
                    entities.Add(BuildGymEntrance(user));
                }
            }
            else if (isKurumsal)
            {
                if(user.DurationUnit == DurationUnit.Year)
                {
                    entities.Add(BuildGymEntrance(user));
                    entities.Add(BuildSpaSaunaBarcode(user));
                    entities.Add(BuildPrivateLessonBarcode(user, remainingSession: 6));
                }
                else if(user.DurationUnit == DurationUnit.Month)
                {
                    entities.Add(BuildGymEntrance(user));
                    entities.Add(BuildPrivateLessonBarcode(user, remainingSession: 1));
                }
                 else if(user.DurationUnit == DurationUnit.Day)
                {
                    entities.Add(BuildGymEntrance(user));
                }
            }
          return entities;
        }
        private BarcodeEntity BuildGymEntrance(MemberResponse user)
        {
            var memberCode5 = GetValidMemberCode(user.MemberCode);
            var randomSuffix = Random.Shared.Next(10, 99).ToString();

            return new BarcodeEntity
            {
                MemberId = user.Id,
                MemberName = user.FullName,
                MemberCode = memberCode5,
                Types = BarcodeTypes.GymEntrance,
                Code = $"SG{memberCode5}{randomSuffix}", // 9 chars
                Email = user.Email,
                IsActive = true,
                CreatedAt = user.StartDate,
                ExpirationDate = user.EndDate
            };
        }

        private BarcodeEntity BuildSpaSaunaBarcode(MemberResponse user)
        {
            var memberCode5 = GetValidMemberCode(user.MemberCode);
            var randomSuffix = Random.Shared.Next(10, 99).ToString();

            return new BarcodeEntity
            {
                MemberId = user.Id,
                MemberName = user.FullName,
                MemberCode = memberCode5,
                Types = BarcodeTypes.SpaSauna,
                Code = $"SP{randomSuffix}", // 4 chars
                Email = user.Email,
                IsActive = true,
                CreatedAt = user.StartDate,
                ExpirationDate = user.EndDate
            };
        }

        private BarcodeEntity BuildPrivateLessonBarcode(MemberResponse user, int remainingSession)
        {
            var memberCode5 = GetValidMemberCode(user.MemberCode);

            return new BarcodeEntity
            {
                MemberId = user.Id,
                MemberName = user.FullName,
                MemberCode = memberCode5,
                Types = BarcodeTypes.PrivateLesson,
                Code = $"{memberCode5}{remainingSession:D3}", // 8 chars
                Email = user.Email,
                IsActive = true,
                CreatedAt = user.StartDate,
                ExpirationDate = user.EndDate
            };
        }

        private BarcodeEntity BuildGroupLessonBarcode(MemberResponse user)
        {
            var memberCode5 = GetValidMemberCode(user.MemberCode);
            var randomSuffix = Random.Shared.Next(10, 99).ToString();

            return new BarcodeEntity
            {
                MemberId = user.Id,
                MemberName = user.FullName,
                MemberCode = memberCode5,
                Types = BarcodeTypes.GroupLesson,
                Code = $"GD{memberCode5}{randomSuffix}",
                Email = user.Email,
                IsActive = true,
                CreatedAt = user.StartDate,
                ExpirationDate = user.EndDate
            };
        }
        private static string GetValidMemberCode(string? memberCode)
        {
            var clean = (memberCode ?? "ST101").Replace("-", "").Trim().ToUpperInvariant();
            if (clean.Length == 5) return clean;
            return clean.Length > 5 ? clean[..5] : clean.PadRight(5, '0');
        }

        public Task<IEnumerable<BarcodeResponse>> DeleteBarcodeById(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<BarcodeResponse>> DeleteBarcodeByEmail(string Email)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<BarcodeResponse>> DeleteBarcodeByMemberId(Guid id)
        {
            throw new NotImplementedException();
        }
   
        public async Task<IEnumerable<BarcodeResponse>> GetBarcodeByEmail(string email)
        {
            var barcodes = await _barcodeRepo.GetBarcodeMyMemberEmail(email);
            return barcodes.ToResponseList();
        }

        public async Task<IEnumerable<BarcodeResponse>> GetBarcodeByMemberId(Guid memberId)
        {
            var barcodes = await _barcodeRepo.GetActiveBarcodesByMemberIdAsync(memberId);
            return barcodes.ToResponseList();
        }

        public async Task<BarcodeResponse> GetBarcodeById(int id)
        {
            var barcode = await _barcodeRepo.GetByBarcodeIdAsync(id);
            return barcode?.ToBarcodeResponse()!;
        }

        public async Task<IEnumerable<BarcodeResponse>> GetAllBarcodes()
        {
            var barcodes = await _barcodeRepo.GetAllBarcodes();
            return barcodes.ToResponseList();
        }
    }
}