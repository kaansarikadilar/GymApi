using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GymApi.DTOs;
using GymApi.Models;
using Npgsql.Replication;

namespace GymApi.Mappers
{
    public static class AppUserMapper
    {
        public static AppUserResponse ToAppUserResponse(this AppUser user,string role)
        {
            return new AppUserResponse
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email!,
                Role = role
            };
        }
        public static AuthResponse ToAuthResponse(this AppUser user,string token,string role)
        {
            return new AuthResponse
            {
                Token = token,
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email!,
                Role = role
            };
        }
    }
}