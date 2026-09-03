using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GymApi.Modules.Barcode.Models;

namespace GymApi.Modules.Barcode.DTOs
{
    public class BarcodeResponse
    {
    public int Id { get; set; } 
    public string MemberName { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string Email { get; set; } =string.Empty;
    public BarcodeTypes BarcodeType { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime ExpirationDate { get; set; }
    public bool IsActive { get; set; }
    public string MemberCode { get; set; } = string.Empty;
    }
}