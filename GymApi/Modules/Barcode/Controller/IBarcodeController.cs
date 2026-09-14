using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GymApi.Helpers;
using GymApi.Modules.Barcode.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ActionConstraints;

namespace GymApi.Modules.Barcode.Controller
{
    public interface IBarcodeController
    {
        Task<IActionResult> ManualBarcodeGeneration([FromBody]BarcodeRequest request);
        Task<IActionResult> BarcodeUpdate([FromBody]BarcodeUpdateRequest request);
        Task<IActionResult> BarcodeUpdateByMember([FromQuery] string Email);
        Task<IActionResult> GetAllBarcodes([FromQuery]BarcodeQueryObject query);
        Task<IActionResult> DeleteBarcodeById(int id);
        Task<IActionResult> DeleteBarcodeByEmail(string Email);
        Task<IActionResult> DeleteBarcodeByMemberId(Guid id);
        Task<IActionResult> GetBarcodeByMemberId(Guid id);
        Task<IActionResult> GetBarcodeById(int id);
        Task<IActionResult> GetBarcodeByMemberEmail(string Email);
    }
}