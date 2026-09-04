using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GymApi.Modules.Barcode.DTOs;
using GymApi.Modules.Barcode.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GymApi.Modules.Barcode.Controller.BarcodeControllerImpl
{
    [ApiController]
    [Route("/GymApi/BarcodeController")]
    [Authorize]
    public class BarcodeControllerImpl : ControllerBase, IBarcodeController
    {
        private readonly IBarcodeService _barcodeService;

        public BarcodeControllerImpl(IBarcodeService barcodeService)
        {
            _barcodeService = barcodeService;
        }

        [HttpPost("generate")]
        public async Task<IActionResult> BarcodeGeneration(string Email)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var barcode = await _barcodeService.BarcodeGeneration(Email);
            if (barcode == null || !barcode.Any())
            {
                return NotFound("Cannot create barcode. Member not found or requested barcode type is not allowed for this membership."); 
            }

            return Ok(barcode);
        }
        [HttpDelete("Email")]
        public async Task<IActionResult> DeleteBarcodeByEmail(string Email)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var response = await _barcodeService.DeleteBarcodeByEmail(Email);
            if (response == false)
            {
                return NotFound("Cannot find Barcode");
            }

            return Ok(new { message = $"Barcode '{Email}' deleted successfully." });
        }
        [HttpDelete("BarcodeId")]

        public async Task<IActionResult> DeleteBarcodeById(int id)
        {
             if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await _barcodeService.DeleteBarcodeById(id);
            if (response == false)
            {
                return NotFound("Cannot find Barcode");
            }

            return Ok(new { message = $"Barcode '{id}' deleted successfully." });
        }
        [HttpDelete("MemberId")]

        public async Task<IActionResult> DeleteBarcodeByMemberId(Guid id)
        {
             if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await _barcodeService.DeleteBarcodeByMemberId(id);
            if (response == false)
            {
                return NotFound("Cannot find Barcode");
            }
            return Ok(new { message = $"Barcode '{id}' deleted successfully." });
        }

        [HttpGet("All")]
        public async Task<IActionResult> GetAllBarcodes()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var barcodes = await _barcodeService.GetAllBarcodes();
            return Ok(barcodes ?? Enumerable.Empty<BarcodeResponse>());
        }

        [HttpGet("Id")]
        public async Task<IActionResult> GetBarcodeById(int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await _barcodeService.GetBarcodeById(id);
            if (response == null)
            {
                return NotFound("Cannot find Barcode");
            }

            return Ok(response);
        }

        [HttpGet("ByMemberEmail")]
        public async Task<IActionResult> GetBarcodeByMemberEmail(string Email)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await _barcodeService.GetBarcodeByEmail(Email);
            // Having 0 barcodes is valid for a new member — returns 200 OK with []
            return Ok(response ?? Enumerable.Empty<BarcodeResponse>());
        }

        [HttpGet("ByMemberId")]
        public async Task<IActionResult> GetBarcodeByMemberId(Guid id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await _barcodeService.GetBarcodeByMemberId(id);
            // Having 0 barcodes is valid — returns 200 OK with []
            return Ok(response ?? Enumerable.Empty<BarcodeResponse>());
        }

        [HttpPost("manual")]
        public async Task<IActionResult> ManualBarcodeGeneration([FromBody] BarcodeRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var barcode = await _barcodeService.ManualBarcodeCreation(request);
            if (barcode == null || !barcode.Any())
            {
                return NotFound("Cannot create barcode. Member not found or requested barcode type is not allowed for this membership.");  
            }

            return Ok(barcode);
        }
    }
}