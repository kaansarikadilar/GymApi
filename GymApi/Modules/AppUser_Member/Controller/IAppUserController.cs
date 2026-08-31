using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GymApi.DTOs;
using GymApi.DTOs.AppUser;
using GymApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ActionConstraints;

namespace GymApi.Controller
{
    public interface IAppUserController
    {
    Task<IActionResult> Register([FromBody] AppUserRequest request);
    Task<IActionResult> Login([FromBody] AppUserLogin request);
    Task<IActionResult> Delete(string email);
    Task<IActionResult> Update([FromBody]AppUserUpdateRequest request);
    }
}