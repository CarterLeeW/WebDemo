using System;
using API.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

public class AdminController(UserManager<AppUser> userManager) : BaseApiController
{
    [Authorize(Policy = "RequireAdminRole")]
    [HttpGet("users-with-roles")]
    public async Task<ActionResult> GetUsersWithRoles()
    {
        // return list of users with id, username, and roles they are in
        var users = await userManager.Users
            .OrderBy(x => x.UserName)
            .Select(x => new 
            {
                x.Id,
                Username = x.UserName,
                Roles = x.UserRoles.Select(r => r.Role.Name).ToList()
            }).ToListAsync();
        return Ok(users);
    }

    [Authorize(Policy = "RequireAdminRole")]
    [HttpPost("edit-roles/{username}")]
    public async Task<ActionResult> EditRoles(string username, string roles)
    {
        if (string.IsNullOrEmpty(roles)) return BadRequest("you must select at least one role");

        var selectedRoles = roles.Split(",").ToArray();

        var user = await userManager.FindByNameAsync(username);
        if (user == null) return BadRequest("User not found");

        var currUserRoles = await userManager.GetRolesAsync(user);

        // add roles
        var result = await userManager.AddToRolesAsync(user, selectedRoles.Except(currUserRoles));
        if (!result.Succeeded) return BadRequest("Failed to add to roles");
        // keeps only the roles that are passed in
        result = await userManager.RemoveFromRolesAsync(user, currUserRoles.Except(selectedRoles));
        if (!result.Succeeded) return BadRequest("Failed to remove from roles");
        // return updated list of roles
        return Ok(await userManager.GetRolesAsync(user));
    }

    [Authorize(Policy = "ModeratePhotoRole")]
    [HttpGet("photos-to-moderate")]
    public ActionResult GetPhotosForModeration()
    {
        return Ok("Only admins or moderators can see this");
    }
}
