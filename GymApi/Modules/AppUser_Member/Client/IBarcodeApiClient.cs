using System.Collections.Generic;
using System.Threading.Tasks;
using GymApi.Modules.Barcode.DTOs;
using Microsoft.AspNetCore.Mvc;
using Refit;

namespace GymApi.Modules.Barcode.Clients
{
    public interface IBarcodeApiClient
    {
       [Post("/GymApi/BarcodeController/generate")]
        Task<IEnumerable<BarcodeResponse>> BarcodeGeneration(
            [Query] string email, 
            [Header("Authorization")] string token);
    }
}