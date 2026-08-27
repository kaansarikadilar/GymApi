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
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ITokenService _tokenService;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ApplicationDbContext _dbContext;

        public AppUserControllerImpl(
            UserManager<AppUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ITokenService tokenService,
            SignInManager<AppUser> signInManager,
            ApplicationDbContext dbContext)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _tokenService = tokenService;
            _signInManager = signInManager;
            _dbContext = dbContext;
        }
        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] AppUserRequest request)
        {
            if (!ModelState.IsValid){
                return BadRequest(ModelState);
            }
            var existingUser = await _userManager.FindByEmailAsync(request.Email);
            if (existingUser != null){
                return BadRequest(new { message = "Email is already in use." });
            }
            var user = new AppUser
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                UserName = request.Email
            };
            var result = await _userManager.CreateAsync(user, request.Password);
            if (!result.Succeeded){
                return BadRequest(result.Errors);
            }
            // Verify and assign requested role ("Admin", "Trainer", "User")
            var roleExists = await _roleManager.RoleExistsAsync(request.Role);
            if (!roleExists){
                return BadRequest(new { message = $"Role '{request.Role}' does not exist." });
            }
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
            var existingUser = await _userManager.FindByEmailAsync(email);
            if (existingUser == null)
            {
               return NotFound(new { message = "User account not found." });
            }
            var result = await _signInManager.CheckPasswordSignInAsync(existingUser, request.ExistingPassword, false);
            {
                if (!result.Succeeded)
                {
                    return Unauthorized(new { message = "Invalid email or password." });
                }
            }
            if(!string.IsNullOrEmpty(request.NewPassword))
            {
                var resultPasword =  await _userManager.ChangePasswordAsync(existingUser,request.ExistingPassword,request.NewPassword!);
                if (!resultPasword.Succeeded)
                {
                    return BadRequest(resultPasword.Errors);
                }
            }
            var roleExists = await _roleManager.RoleExistsAsync(request.Role);
            if(!roleExists!)
            {
                return BadRequest(new { message = $"Role '{request.Role}' does not exist." });
            }
            existingUser.FirstName = request.FirstName;
            existingUser.LastName = request.LastName;
             if (!string.IsNullOrEmpty(request.NewEmail)){
            existingUser.Email = request.NewEmail;
            }
            var updateResult = await _userManager.UpdateAsync(existingUser);
            await _userManager.AddToRoleAsync(existingUser, request.Role);
            if (!updateResult.Succeeded)
            {
                return BadRequest(updateResult.Errors);
            }
            var response = new AppUserResponse
            {
                Id = existingUser.Id,
                FirstName =existingUser.FirstName,
                LastName = existingUser.LastName,
                Email = existingUser.Email!,
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
