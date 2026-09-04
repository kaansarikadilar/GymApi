using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GymApi.Data;
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
            .Where(a=>a.Email == Email)
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
         public async Task<IEnumerable<BarcodeEntity>> GetAllBarcodes()
        {
            return await _barcodeContext.Barcodes
            .AsNoTracking()
            .AsAsyncEnumerable()
            .ToListAsync();
        }
        public async Task<BarcodeEntity> UpdateBarcodeAsync(BarcodeEntity barcode)
        {
            _barcodeContext.Barcodes.Update(barcode);
            await _barcodeContext.SaveChangesAsync();
            return barcode;
        }
    }
}