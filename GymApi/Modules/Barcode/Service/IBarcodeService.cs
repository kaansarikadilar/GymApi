using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GymApi.Models;
using GymApi.Modules.Barcode.DTOs;

namespace GymApi.Modules.Barcode.Service
{
    public interface IBarcodeService
    {
        Task<IEnumerable<BarcodeResponse>> ManualBarcodeCreation(BarcodeRequest request);
        Task<IEnumerable<BarcodeResponse?>> BarcodeGeneration(string mail);
        Task<IEnumerable<BarcodeResponse>> DeleteBarcodeById(int id);
        Task<IEnumerable<BarcodeResponse>> DeleteBarcodeByEmail(string Email);
        Task<IEnumerable<BarcodeResponse>> DeleteBarcodeByMemberId(Guid id);
        Task<IEnumerable<BarcodeResponse>> GetAllBarcodes();
        Task<IEnumerable<BarcodeResponse>> GetBarcodeByMemberId(Guid id);
        Task<BarcodeResponse> GetBarcodeById(int id);
        Task<IEnumerable<BarcodeResponse>>GetBarcodeByEmail(string Email);
    }
}