using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GymApi.DTOs.Member;
using GymApi.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace GymApi.Controller
{
    public interface IMemberController
    {
       Task<IActionResult>CreateMember([FromBody]MemberRequest request); 
       Task<IActionResult>DeleteMember(string mail); 
       Task<IActionResult>DeleteMemberBarcode(string mail);
       Task<IActionResult>GetAllMembers([FromQuery]MemberQueryObject queryObject); 
       Task<IActionResult>GetMemberByEmail(string mail); 
       Task<IActionResult>GetMemberById(Guid id); 
       Task<IActionResult>UpdateMember(string mail,[FromBody]UpdateMemberRequest request); 
    }
}