using System.Security.Cryptography;
using System.Text;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

public class AccountController(DataContext context, ITokenService tokenService, IMapper mapper) : BaseApiController
{
    [HttpPost("register")] // account/register
    public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto) // use [FromQuery] or [FromBody] for http parameters, strings work for query
    {
        if (await UserExists(registerDto.Username))
        {
            return BadRequest("Username is taken");
        }

        using var hmac = new HMACSHA512();

        var user = mapper.Map<AppUser>(registerDto);
        user.UserName = registerDto.Username.ToLower();

        context.Users.Add(user);
        await context.SaveChangesAsync();

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
        var user = await context.Users
            .Include(p => p.Photos)
            .FirstOrDefaultAsync(x => x.UserName == loginDto.Username.ToLower());

        if (user == null)
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
        return await context.Users.AnyAsync(x => x.UserName.ToLower() == username.ToLower()); // lower case only
    }
}
