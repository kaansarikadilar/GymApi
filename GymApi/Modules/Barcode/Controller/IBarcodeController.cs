using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GymApi.Modules.Barcode.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace GymApi.Modules.Barcode.Controller
{
    public interface IBarcodeController
    {
        Task<IActionResult>ManualBarcodeGeneration([FromBody]BarcodeRequest request);
        Task<IActionResult>BarcodeGeneration(string Email);
    }
}