using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GymApi.Modules.Barcode.DTOs;
using Refit;

namespace GymApi.Modules.Barcode.Clients
{
    public interface IBarcodeApiClient
    {
        [Post("/GymApi/BarcodeController/generate")]
        Task<IEnumerable<BarcodeResponse>> BarcodeGeneration(
            [Query] string email, 
            [Header("Authorization")] string token);

        [Get("/GymApi/BarcodeController/All")]
        Task<IEnumerable<BarcodeResponse>> GetAllBarcodes(
            [Header("Authorization")] string token);

        [Get("/GymApi/BarcodeController/Id")]
        Task<IEnumerable<BarcodeResponse>> GetBarcodeById(
            [Query] int id, 
            [Header("Authorization")] string token);

        [Get("/GymApi/BarcodeController/ByMemberEmail")]
        Task<IEnumerable<BarcodeResponse>> GetBarcodeByMemberEmail(
            [Query] string email, 
            [Header("Authorization")] string token);

        [Get("/GymApi/BarcodeController/ByMemberId")]
        Task<IEnumerable<BarcodeResponse>> GetBarcodeByMemberId(
            [Query] Guid id, 
            [Header("Authorization")] string token);

        [Post("/GymApi/BarcodeController/manual")]
        Task<IEnumerable<BarcodeResponse>> ManualBarcodeGeneration(
            [Body] BarcodeRequest request, 
            [Header("Authorization")] string token);
    }
}