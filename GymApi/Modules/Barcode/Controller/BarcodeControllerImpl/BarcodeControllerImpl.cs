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
    public class BarcodeControllerImpl : ControllerBase , IBarcodeController
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
            if(barcode == null || !barcode.Any())
            {
                return NotFound("Cannot create barcode. Member not found or requested barcode type is not allowed for this membership."); 
            }
            return Ok(barcode);
        }
        [HttpPost("manual")]
        public async Task<IActionResult> ManualBarcodeGeneration([FromBody]BarcodeRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
          var barcode = await _barcodeService.ManualBarcodeCreation(request);
            if(barcode == null || !barcode.Any())
            {
                return NotFound("Cannot create barcode. Member not found or requested barcode type is not allowed for this membership.");  
            }
            return Ok(barcode);
        }
    }
}