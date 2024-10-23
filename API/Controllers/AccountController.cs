using System.Security.Cryptography;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

public class AccountController(UserManager<AppUser> userManager, ITokenService tokenService, IMapper mapper) : BaseApiController
{
    [HttpPost("register")] // account/register
    public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto) // use [FromQuery] or [FromBody] for http parameters, strings work for query
    {
        if (await UserExists(registerDto.Username))
        {
            return BadRequest("Username is taken");
        }

        var user = mapper.Map<AppUser>(registerDto);
        user.UserName = registerDto.Username.ToLower();

        var result = await userManager.CreateAsync(user, registerDto.Password);
        if (!result.Succeeded)
        {
            return BadRequest(result.Errors);
        }

        return new UserDto
        {
            Username = user.UserName,
            KnownAs = user.KnownAs,
            Token = tokenService.CreateToken(user),
            Gender = user.Gender
        };
    }

    [HttpPost("login")]
    public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
    {
        var user = await userManager.Users
            .Include(p => p.Photos)
            .FirstOrDefaultAsync(x => x.NormalizedUserName == loginDto.Username.ToUpper());

        if (user == null || user.UserName == null)
        {
            return Unauthorized("Invalid username or password");
        }

        var result = await userManager.CheckPasswordAsync(user, loginDto.Password);
        if (!result)
        {
            return Unauthorized("Invalid username or password");
        }

        return new UserDto
        {
            Username = user.UserName,
            KnownAs = user.KnownAs,
            Token = tokenService.CreateToken(user),
            PhotoUrl = user.Photos.FirstOrDefault(x => x.IsMain)?.Url,
            Gender = user.Gender
        };
    }

    // helper functions
    private async Task<bool> UserExists(string username)
    {
        return await userManager.Users.AnyAsync(x => x.NormalizedUserName == username.ToUpper()); // lower case only
    }
}
