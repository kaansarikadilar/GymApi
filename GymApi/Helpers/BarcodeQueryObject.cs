using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GymApi.Helpers
{
    public class BarcodeQueryObject
    {
    public bool? IsActive { get; set; }
    public string CreatedAt { get; set; } = string.Empty;
    public string ExpirationDate { get; set; } = string.Empty;
    public string? SortBy { get; set; } = null;
    public bool IsDescending { get; set; } = false;
    public int PageNumber { get; set; } = 1;        
    public int PageSize { get; set; } = 10;
    }
}