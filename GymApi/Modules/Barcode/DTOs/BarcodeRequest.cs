using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using GymApi.Modules.Barcode.Models;

namespace GymApi.Modules.Barcode.DTOs
{
    public class BarcodeRequest
    {
        [Required]
        public string Email { get; set; } = string.Empty;
        public BarcodeTypes? BarcodeType { get; set; }  // e.g., "Gym", "Spa"
    }
}