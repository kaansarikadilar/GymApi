using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GymApi.Helpers;
using GymApi.Modules.Barcode.Models;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;

namespace GymApi.Modules.Barcode.Repository
{
    public interface IBarcodeRepository
    {
        Task<IEnumerable<BarcodeEntity>> GetActiveBarcodesByMemberIdAsync(Guid memberId);
        Task<IEnumerable<BarcodeEntity>>GetBarcodeMyMemberEmail(string Email);
        Task<BarcodeEntity> AddBarcodeAsync(BarcodeEntity barcodeEntity);
        Task<bool> DeactivateBarcodeByEmail(string Email);
        Task<bool> DeleteBarcodeById(int id);
        Task<bool> DeleteBarcodeByEmail(string Email);
        Task<bool> DeleteBarcodeByMemberId(Guid id);
        Task<BarcodeEntity>GetByBarcodeIdAsync(int id);
        Task<IEnumerable<BarcodeEntity>>GetAllBarcodes(BarcodeQueryObject query);
        Task<IEnumerable<BarcodeEntity>>GetBarcodeByMemberId(Guid id);
    }
}