using System;
using System.Threading.Tasks;
using GymApi.DTOs.Member;
using GymApi.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;

namespace GymApi.Controller.Impl
{
    [ApiController]
    [Route("api/members")]
    public class MemberControllerImpl : ControllerBase, IMemberController
    {
        private readonly IMemberService _memberService;

        public MemberControllerImpl(IMemberService memberService)
        {
            _memberService = memberService;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateMember([FromBody] MemberRequest request)
        {
            var creatingUser = await _memberService.CreateMemberAsync(request);
            if (creatingUser == null)
            {
                return BadRequest("User cannot be created.");
            }
            return CreatedAtAction(nameof(GetMemberById), new { id = creatingUser.Id }, creatingUser);
        }
        [HttpPut("by-email/{email}")]
        [Authorize]
         public async Task<IActionResult> UpdateMember(string email, [FromBody] UpdateMemberRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var updatingMember = await _memberService.UpdateMemberAsync(email,request);
            if(updatingMember == null)
            {
                return NotFound($"User '{email}' could not be found or updated.");
            }
            return CreatedAtAction(nameof(GetMemberById), new { id = updatingMember.Id }, updatingMember);
        }
        [HttpDelete("by-email/{email}")]
        [Authorize(Roles ="Admin")]
        public async Task<IActionResult> DeleteMember(string email)
        {
            var isDeleted = await _memberService.DeleteMemberAsync(email);
            if (!isDeleted)
            {
                return NotFound($"User '{email}' could not be found or deleted.");
            }
            return Ok(new { message = $"User '{email}' deleted successfully." });
        }
        [HttpGet("All")]
        public async Task<IActionResult> GetAllMembers()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var AllUsers = await _memberService.GetAllMembersAsync();
            if(AllUsers == null)
            {
                return NotFound("Users cannot be found");
            }
            return Ok(AllUsers);
        }

        [HttpGet("by-email/{email}")]
        [Authorize]
        public async Task<IActionResult> GetMemberByEmail(string email)
        {
            var user = await _memberService.GetMemberByEmail(email);
            if (user == null)
            {
                return NotFound("User cannot be found.");
            }
            return Ok(user);
        }
        [HttpGet("{id:guid}")]
        [Authorize]
        public async Task<IActionResult> GetMemberById(Guid id)
        {
            var user = await _memberService.GetByIdAsync(id);
            if (user == null)
            {
                return NotFound("User cannot be found.");
            }
            return Ok(user);
        }
    }
}