using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

[Authorize]
public class UsersController(IUserRepository userRepository, IMapper mapper) : BaseApiController
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<MemberDto>>> GetUsers()
    {
        var users = await userRepository.GetUsersAsync();

        return Ok(mapper.Map<IEnumerable<MemberDto>>(users));
    }

    [HttpGet("{username}")] // /api/users/{id}
    public async Task<ActionResult<MemberDto>> GetUser(string username)
    {
        var user = await userRepository.GetUserByUsernameAsync(username);
        
        if (user == null)
        {
            return NotFound();
        }


        return mapper.Map<MemberDto>(user);
    }
}
