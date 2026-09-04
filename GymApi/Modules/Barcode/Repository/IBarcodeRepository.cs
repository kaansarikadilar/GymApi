using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GymApi.Modules.Barcode.Models;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;

namespace GymApi.Modules.Barcode.Repository
{
    public interface IBarcodeRepository
    {
        Task<IEnumerable<BarcodeEntity>> GetActiveBarcodesByMemberIdAsync(Guid memberId);
        Task<IEnumerable<BarcodeEntity>>GetBarcodeMyMemberEmail(string Email);
        Task<BarcodeEntity> AddBarcodeAsync(BarcodeEntity barcodeEntity);
        Task<BarcodeEntity> UpdateBarcodeAsync(BarcodeEntity barcodeEntity);
        Task<bool> DeleteBarcodeById(int id);
        Task<bool> DeleteByMemberId(Guid id);
        Task<BarcodeEntity>GetByBarcodeIdAsync(int id);
        Task<IEnumerable<BarcodeEntity>>GetAllBarcodes();
        Task<IEnumerable<BarcodeEntity>>GetBarcodeByMemberId(Guid id);
    }
}