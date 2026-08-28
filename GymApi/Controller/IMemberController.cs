using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GymApi.DTOs.Member;
using Microsoft.AspNetCore.Mvc;

namespace GymApi.Controller
{
    public interface IMemberController
    {
       public Task<IActionResult>CreateMember([FromBody]MemberRequest request); 
       public Task<IActionResult>DeleteMember(string mail); 
       public Task<IActionResult>GetAllMembers(); 
       public Task<IActionResult>GetMemberByEmail(string mail); 
       public Task<IActionResult>GetMemberById(Guid id); 
       public Task<IActionResult>UpdateMember(string mail,[FromBody]UpdateMemberRequest request); 
    }
}