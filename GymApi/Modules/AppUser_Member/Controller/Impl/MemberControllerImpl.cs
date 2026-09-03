using System;
using System.Threading.Tasks;
using GymApi.DTOs.Member;
using GymApi.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GymApi.Controller.Impl
{
    [ApiController]
    [Authorize]
    [Route("api/members")]
    public class MemberControllerImpl : ControllerBase, IMemberController
    {
        private readonly IMemberService _memberService;

        public MemberControllerImpl(IMemberService memberService)
        {
            _memberService = memberService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateMember([FromBody] MemberRequest request)
        {
            var creatingUser = await _memberService.CreateMemberAsync(request);
            if (creatingUser == null)
            {
                return BadRequest("User cannot be created.");
            }
            return CreatedAtAction(nameof(GetMemberById), new { id = creatingUser.Id }, creatingUser);
        }

        [HttpPut("by-email")]
        public async Task<IActionResult> UpdateMember([FromQuery] string email, [FromBody] UpdateMemberRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var updatingMember = await _memberService.UpdateMemberAsync(email, request);
            if (updatingMember == null)
            {
                return NotFound($"User '{email}' could not be found or updated.");
            }
            return CreatedAtAction(nameof(GetMemberById), new { id = updatingMember.Id }, updatingMember);
        }

        [HttpDelete("by-email")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteMember([FromQuery] string email)
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
            var allUsers = await _memberService.GetAllMembersAsync();
            if (allUsers == null)
            {
                return NotFound("Users cannot be found");
            }
            return Ok(allUsers);
        }

        [HttpGet("by-email")]
        public async Task<IActionResult> GetMemberByEmail([FromQuery] string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return BadRequest("Email parameter is required.");
            }

            var user = await _memberService.GetMemberByEmail(email);
            if (user == null)
            {
                return NotFound("User cannot be found.");
            }
            return Ok(user);
        }

        [HttpGet("{id:guid}")]
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