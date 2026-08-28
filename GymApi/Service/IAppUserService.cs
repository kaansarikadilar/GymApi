using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GymApi.DTOs;
using GymApi.DTOs.AppUser;
using GymApi.Models;

namespace GymApi.Service
{
    public interface IAppUserService
    {
        public Task<AppUserResponse?> RegisterUser(AppUserRequest request);
        public Task<AuthResponse?> LogInUser(AppUserLogin requestLogin);
        public Task<AppUserResponse?> UpdateUser(string claimEmail,AppUserUpdateRequest updateRequest);
        public Task<AppUserResponse?> DeleteUser(string mail);
    }
}