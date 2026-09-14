using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GymApi.Data;
using GymApi.Helpers;
using GymApi.Modules.Barcode.Clients;
using GymApi.Modules.Barcode.Models;
using GymApi.Service;
using Microsoft.EntityFrameworkCore;

namespace GymApi.Modules.Barcode.Repository
{
    public class BarcodeRepositoryImpl : IBarcodeRepository
    {
        private readonly BarcodeDbContext _barcodeContext;
        public BarcodeRepositoryImpl(BarcodeDbContext barcodeContext)
        {
            _barcodeContext = barcodeContext;
        }
        public async Task<BarcodeEntity> AddBarcodeAsync(BarcodeEntity barcode )
        {
            await _barcodeContext.AddAsync(barcode);
            await _barcodeContext.SaveChangesAsync();
            return barcode;
        }
        public async Task<IEnumerable<BarcodeEntity>> GetActiveBarcodesByMemberIdAsync(Guid memberId)
        {
            return await _barcodeContext.Barcodes
            .Where(b => b.MemberId == memberId && b.IsActive && b.ExpirationDate > DateTime.UtcNow)
            .ToListAsync();
        }
        public async Task<bool> DeleteBarcodeByMemberId(Guid id)
        {
            var member = await _barcodeContext.Barcodes.Where(b=>b.MemberId == id).ToListAsync();
            if (!member.Any())
            {
                return false;
            }
                _barcodeContext.RemoveRange(member);
                await _barcodeContext.SaveChangesAsync();
                return true;
        }
        public async Task<bool> DeleteBarcodeByEmail(string Email)
        {
            var barcode = await _barcodeContext.Barcodes.Where(a=>a.Email == Email).ToListAsync();
            if(!barcode.Any())
            {
                return false;
            }
            _barcodeContext.RemoveRange(barcode);
            await _barcodeContext.SaveChangesAsync();
            return true;
        }
        public async Task<bool> DeleteBarcodeById(int id)
        {
            var barcode = await _barcodeContext.Barcodes.FindAsync(id);
            if(barcode == null)
            {
                return false;
            }
            _barcodeContext.Remove(barcode);
            await _barcodeContext.SaveChangesAsync();
            return true;
        }
        public async Task<IEnumerable<BarcodeEntity>> GetBarcodeByMemberId(Guid id)
        {
            return await _barcodeContext.Barcodes
            .AsNoTracking()
            .Where(b=>b.MemberId == id)
            .ToListAsync();
        }
         public async Task<IEnumerable<BarcodeEntity>> GetBarcodeMyMemberEmail(string Email)
        {
            return await _barcodeContext.Barcodes
            .AsNoTracking()
            .Where(a=>a.Email == Email && a.IsActive)
            .ToListAsync();
        }
        public async Task<BarcodeEntity> GetByBarcodeIdAsync(int id)
        {
            var barcodes = await _barcodeContext.Barcodes.FindAsync(id);
            if (barcodes == null)
            {
                return null!;
            }
            return barcodes;
        }
        public async Task<IEnumerable<BarcodeEntity>> GetAllBarcodes(BarcodeQueryObject query)
        {
        var barcodes = _barcodeContext.Barcodes.AsNoTracking().AsQueryable();

        // 1. Filter by IsActive without dropping AsNoTracking()
        if (query.IsActive.HasValue)
        {
            barcodes = barcodes.Where(a => a.IsActive == query.IsActive.Value);
        }
            // 2. Sorting (with a reliable default fallback when SortBy is empty)
            if (!string.IsNullOrWhiteSpace(query.SortBy))
            {
                if (query.SortBy.Equals("IsActive", StringComparison.OrdinalIgnoreCase))
                {
                    barcodes = query.IsDescending ? barcodes.OrderByDescending(a => a.IsActive) : barcodes.OrderBy(a => a.IsActive);
                }
                else if (query.SortBy.Equals("CreatedAt", StringComparison.OrdinalIgnoreCase))
                {
                    barcodes = query.IsDescending ? barcodes.OrderByDescending(a => a.CreatedAt) : barcodes.OrderBy(a => a.CreatedAt);
                }
                else if (query.SortBy.Equals("ExpirationDate", StringComparison.OrdinalIgnoreCase))
                {
                    barcodes = query.IsDescending ? barcodes.OrderByDescending(a => a.ExpirationDate) : barcodes.OrderBy(a => a.ExpirationDate);
                }
                else
                {
                    barcodes = barcodes.OrderByDescending(a => a.CreatedAt);
                }
            }
            else
            {
                barcodes = barcodes.OrderByDescending(a => a.CreatedAt);
            }
            if (query.PageSize <= 0 || query.PageNumber <= 0)
            {
                return await barcodes.ToListAsync();
            }

            var skipNumber = (query.PageNumber - 1) * query.PageSize;
            return await barcodes.Skip(skipNumber).Take(query.PageSize).ToListAsync();
        }
        public async Task<bool> DeactivateBarcodeByEmail(string email)
{
            var activeBarcodes = await _barcodeContext.Barcodes
                .Where(b => b.Email == email && b.IsActive)
                .ToListAsync();

            if (activeBarcodes.Any())
            {
                var oneMonthAgo = DateTime.UtcNow.AddMonths(-1);

                foreach (var barcode in activeBarcodes)
                {
                    if (barcode.CreatedAt <= oneMonthAgo)
                    {
                        _barcodeContext.Barcodes.Remove(barcode); // Permanent delete after 1 month
                    }
                    else
                    {
                        barcode.IsActive = false; 
                    }
                }

                await _barcodeContext.SaveChangesAsync();
            }

            return true; // Always return true so the update flow doesn't abort
        }
    }
}