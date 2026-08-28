using System.Reflection.Metadata.Ecma335;
using System.Security.Claims;
using GymApi.Data;
using GymApi.DTOs;
using GymApi.DTOs.AppUser;
using GymApi.Models;
using GymApi.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace GymApi.Controller.Impl
{
    [ApiController]
    [Route("/GymApi/AppUserController")]
    public class AppUserControllerImpl : ControllerBase,IAppUserController
    {
        private readonly IAppUserService _appUserService;
        public AppUserControllerImpl(IAppUserService appUserService)
        {
            _appUserService = appUserService;
        }
        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] AppUserRequest request)
        {
            if (!ModelState.IsValid){
                return BadRequest(ModelState);
            }
            var createUser = await _appUserService.RegisterUser(request);
            if(createUser == null)
            {
                return BadRequest(new { message = "Registering Failed" });
            }
            return Ok(createUser);
        }
        [HttpPut("update")]
        [Authorize]
        public async Task<IActionResult> Update([FromBody]AppUserUpdateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var email = User.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(email))
            {
                return Unauthorized(new { message = "Invalid user token claims." });
            }
            var update = await _appUserService.UpdateUser(email,request);
            if(update == null)
            {
                return BadRequest(new { message = "Updating Failed" });
            }
            return Ok(update);
        }
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] AppUserLogin request)
        {
           if (!ModelState.IsValid)
            {
            return BadRequest(ModelState);
           }
           var LogingIn = await _appUserService.LogInUser(request);
           if(LogingIn == null)
            {
                return Unauthorized(new { message = "Invalid email or password." });           
            }
            return Ok(LogingIn);
        }
        [HttpDelete("{email}")]
        [Authorize(Roles ="Admin")]
        public async Task<IActionResult> Delete(string email)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _appUserService.DeleteUser(email);
            if (result==null)
            {
                return BadRequest(result);
            }

            return NotFound(new { message = $"User '{email}' Deleted." });
        }
    }
}
