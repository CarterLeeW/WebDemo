using System;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using API.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class Seed
{
    public static async Task SeedUsers(DataContext context)
    {
        if (await context.Users.AnyAsync())
        {
            return;
        }

        var userData = await File.ReadAllTextAsync("Data/UserSeedData.json");
        var options = new JsonSerializerOptions{PropertyNameCaseInsensitive = true};
        var users = JsonSerializer.Deserialize<List<AppUser>>(userData, options);

        if (users == null)
        {
            return;
        }

        foreach (AppUser user in users)
        {
            using var hmac = new HMACSHA512();

            // standardize username and create password hash and salt, which are not in the seed data
            user.UserName = user.UserName.ToLower();

            context.Users.Add(user);
        }

        await context.SaveChangesAsync();
    }
}
