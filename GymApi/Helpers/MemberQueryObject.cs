using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GymApi.Helpers
{
    public class MemberQueryObject
    {
    public Guid? Id { get; set; }
    public bool? DoesTrainer { get; set; }
    public bool? IsActive { get; set; }
    public string? Email { get; set; } 
    public string? SortBy { get; set; } = null;
    public bool IsDescending { get; set; } = false;
    public int PageNumber { get; set; } = 1;        
    public int PageSize { get; set; } = 10;
    }
}