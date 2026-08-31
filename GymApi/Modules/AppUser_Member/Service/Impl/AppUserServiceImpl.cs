using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GymApi.Data;
using GymApi.DTOs;
using GymApi.DTOs.AppUser;
using GymApi.Mappers;
using GymApi.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Npgsql.Internal;

namespace GymApi.Service.Impl
{
    public class AppUserServiceImpl : IAppUserService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ITokenService _tokenService;
        private readonly SignInManager<AppUser> _signInManager;

        public AppUserServiceImpl(
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
        public async Task<AppUserResponse?> DeleteUser(string mail)
        {
            var existingUser = await _userManager.FindByEmailAsync(mail);
            
            if(existingUser == null)
            {
                return null;
            }
            var roles = await _userManager.GetRolesAsync(existingUser!);
            var userRole = roles.FirstOrDefault() ?? "User";

            var result = await _userManager.DeleteAsync(existingUser);
            if (!result.Succeeded)
            {
                return null;
            }
            return existingUser.ToAppUserResponse(userRole);
        }
        public async Task<AuthResponse?> LogInUser(AppUserLogin requestLogin)
        {
            var user = await _userManager.FindByEmailAsync(requestLogin.Email);
            if(user == null)
            {
                return null;
            }
            var result = await _signInManager.CheckPasswordSignInAsync(user,requestLogin.Password,false);
            if (!result.Succeeded)
            {
                return null;
            }
            var roles = await _userManager.GetRolesAsync(user);
            var userRole = roles.FirstOrDefault() ?? "User";
            var token = _tokenService.CreateToken(user,userRole); //generating the token

            return user.ToAuthResponse(token,userRole);
        }
        public async Task<AppUserResponse?> RegisterUser(AppUserRequest request)
        {
            var existingUser = await _userManager.FindByEmailAsync(request.Email);
            if(existingUser != null)
            {
            return null;    
            }
            var roleExists = await _roleManager.RoleExistsAsync(request.Role);
            if (!roleExists)

            {
                return null;    
            }
            var user = new AppUser
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                UserName = request.Email
            };

            var result = await _userManager.CreateAsync(user,request.Password);
            if(!result.Succeeded)
            {
                return null;
            }
            await _userManager.AddToRoleAsync(user, request.Role);

            return user.ToAppUserResponse(request.Role);
        }

        public async Task<AppUserResponse?> UpdateUser(string claimEmail, AppUserUpdateRequest request)
        {
          var existingUser = await _userManager.FindByEmailAsync(claimEmail);
            if(existingUser == null)
            {
                return null;
            }
            var result = await _signInManager.CheckPasswordSignInAsync(existingUser,request.ExistingPassword,false);
            if (!result.Succeeded)
            {
                return null;
            }

            if (!string.IsNullOrEmpty(request.NewPassword))
            {
                var resultPassword = await _userManager.ChangePasswordAsync
                (existingUser,request.ExistingPassword,request.NewPassword);
                if (!resultPassword.Succeeded)
                {
                    return null;
                }
            }

            var roleExists = await _roleManager.RoleExistsAsync(request.Role);
            if(!roleExists)
            {
                return null;
            }

            existingUser.FirstName = request.FirstName;
            existingUser.LastName = request.LastName;
            if (!string.IsNullOrEmpty(request.NewEmail))
            {
                existingUser.Email = request.NewEmail;
                existingUser.UserName = request.NewEmail;
            }

            var updateResult = await _userManager.UpdateAsync(existingUser);
            if (!updateResult.Succeeded)
            {
                return null;
            }
            var currentRoles = await _userManager.GetRolesAsync(existingUser);
            await _userManager.RemoveFromRolesAsync(existingUser, currentRoles);
            await _userManager.AddToRoleAsync(existingUser, request.Role);

            return existingUser.ToAppUserResponse(request.Role);
        }
    }
}