using System.Reflection.Metadata.Ecma335;
using GymApi.DTOs;
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
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ITokenService _tokenService;
        private readonly SignInManager<AppUser> _signInManager;

        public AppUserControllerImpl(
            UserManager<AppUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ITokenService tokenService,
            SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _tokenService = tokenService;
            _signInManager = signInManager;
        }
        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] AppUserRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existingUser = await _userManager.FindByEmailAsync(request.Email);
            if (existingUser != null)
                return BadRequest(new { message = "Email is already in use." });

            var user = new AppUser
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                UserName = request.Email
            };

            var result = await _userManager.CreateAsync(user, request.Password);
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            // Verify and assign requested role ("Admin", "Trainer", "User")
            var roleExists = await _roleManager.RoleExistsAsync(request.Role);
            if (!roleExists)
                return BadRequest(new { message = $"Role '{request.Role}' does not exist." });

            await _userManager.AddToRoleAsync(user, request.Role);

            var response = new AppUserResponse
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email!,
                Role = request.Role
            };

            return Ok(response);
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] AppUserLogin request)
        {
           if (!ModelState.IsValid)
            {
            return BadRequest(ModelState);
           }
            var user = await _userManager.FindByEmailAsync(request.Email);
                if (user == null)
                {
                    return Unauthorized(new { message = "Invalid email or password." });
                }
            var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password,false);
                if (!result.Succeeded)
                {
                    return Unauthorized(new { message = "Invalid email or password." });
                }
                // Fetch user role
                var roles = await _userManager.GetRolesAsync(user);
                var userRole = roles.FirstOrDefault() ?? "User";

                // Generate JWE Token
                var token = _tokenService.CreateToken(user,userRole);

            return Ok(new AuthResponse
            {
                Token = token,
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email!,
                Role = userRole
            });
        }
        [HttpDelete("{email}")]
        [Authorize(Roles ="Admin")]
        public async Task<IActionResult> Delete(string email)
        {
            var existingUser = await _userManager.FindByEmailAsync(email);
            if(existingUser == null)
            {
                return NotFound("User cannot be found to delete");
            }
            var result = await _userManager.DeleteAsync(existingUser);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }
            return Ok("Deleted User" + existingUser);
        }
    }
}
