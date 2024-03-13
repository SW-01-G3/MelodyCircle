﻿using MelodyCircle.Models;
using Microsoft.AspNetCore.Identity;

namespace MelodyCircle.Data
{
    public class DataSeeder
    {
        public static async Task SeedData(UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            await SeedRoles(roleManager);
            await SeedAdminUser(userManager);
        }

        private static async Task SeedRoles(RoleManager<IdentityRole> roleManager)
        {
            string[] roleNames = { "Student", "Teacher", "Mod", "Admin" };
            foreach (var roleName in roleNames)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }
        }

        private static async Task SeedAdminUser(UserManager<User> userManager)
        {

            if (await userManager.FindByEmailAsync("admin@melodycircle.pt") == null)
            {
                var user = new User
                {
                    UserName = "admin1",
                    Email = "admin@melodycircle.pt",
                    Name = "Admin1",
                    BirthDate = new DateOnly(2024, 1, 22),
                    Password = "Password-123",
                    NormalizedEmail = "ADMIN@MELODYCIRCLE.PT",
                    EmailConfirmed = true,
                };

                var result = await userManager.CreateAsync(user, "Password-123");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, "Admin");
                }
            }

            if (await userManager.FindByEmailAsync("professor1@melodycircle.pt") == null)
            {
                var user = new User
                {
                    UserName = "professor1",
                    Email = "professor1@melodycircle.pt",
                    Name = "professor1",
                    BirthDate = new DateOnly(2024, 1, 22),
                    Password = "Password-123",
                    NormalizedEmail = "PROFESSOR1@MELODYCIRCLE.PT",
                    EmailConfirmed = true,
                };

                var result = await userManager.CreateAsync(user, "Password-123");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, "Teacher");
                }
            }

            if (await userManager.FindByEmailAsync("professor2@melodycircle.pt") == null)
            {
                var user = new User
                {
                    UserName = "professor2",
                    Email = "professor2@melodycircle.pt",
                    Name = "professor2",
                    BirthDate = new DateOnly(2024, 1, 22),
                    Password = "Password-123",
                    NormalizedEmail = "PROFESSOR2@MELODYCIRCLE.PT",
                    EmailConfirmed = true,
                };

                var result = await userManager.CreateAsync(user, "Password-123");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, "Teacher");
                }
            }
        }
    }
}
