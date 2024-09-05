using System;
using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController(DataContext context) : ControllerBase
{
    [HttpGet]
    public ActionResult<IEnumerable<AppUser>> GetUsers()
    {
        var Users = context.Users.ToList();

        return Users;
    }

    [HttpGet("{id:int}")] // /api/users/{id}
    public ActionResult<AppUser> GetUser(int id)
    {
        var User = context.Users.Find(id);
        if (User == null)
        {
            return NotFound();
        }
        return User;
    }
}
