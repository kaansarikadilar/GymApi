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
        Task<IEnumerable<BarcodeResponse>>ManualBarcodeCreation(BarcodeRequest request);
        Task<IEnumerable<BarcodeResponse?>> BarcodeGeneration(string mail);
    }
}