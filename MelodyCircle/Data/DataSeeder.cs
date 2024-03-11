﻿using MelodyCircle.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Update.Internal;
using SQLitePCL;

namespace MelodyCircle.Data
{
    public class DataSeeder
    {
        public static async Task SeedData(UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            await SeedRoles(roleManager);
            await SeedAdminUser(userManager);
            await SeedModUser(userManager);
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
            byte[] defaultProfilePictureBytes;

            // Open the file stream
            using (FileStream fs = new FileStream("./Images/default_pf.png", FileMode.Open, FileAccess.Read))
            {
                // Initialize byte array with the length of the file
                defaultProfilePictureBytes = new byte[fs.Length];

                // Read the file contents into the byte array
                fs.Read(defaultProfilePictureBytes, 0, defaultProfilePictureBytes.Length);
            }
            if (await userManager.FindByEmailAsync("admin@melodycircle.pt") == null)
            {
                var user = new User
                {
                    UserName = "admin1",
                    Email = "admin@melodycircle.pt",
                    Name = "Admin1",
                    BirthDate = new DateOnly(2002, 1, 22),
                    Password = "Password-123",
                    NormalizedEmail = "ADMIN@MELODYCIRCLE.PT",
                    EmailConfirmed = true,
                    Gender = Gender.Male,
                    ProfilePicture = defaultProfilePictureBytes,
                    Locality = "Portugal",
                    Connections = new List<User>(),
                    Ratings = new List<int>()
                };

                var result = await userManager.CreateAsync(user, "Password-123");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, "Admin");
                }
            }

        }

        private static async Task SeedModUser(UserManager<User> userManager)
        {
            byte[] defaultProfilePictureBytes;

            // Open the file stream
            using (FileStream fs = new FileStream("./Images/default_pf.png", FileMode.Open, FileAccess.Read))
            {
                defaultProfilePictureBytes = new byte[fs.Length];
                fs.Read(defaultProfilePictureBytes, 0, defaultProfilePictureBytes.Length);
            }
            if (await userManager.FindByEmailAsync("mod@melodycircle.pt") == null)
            {
                var user = new User
                {
                    UserName = "mod1",
                    Email = "mod@melodycircle.pt",
                    Name = "Moderator1",
                    BirthDate = new DateOnly(2002, 1, 22),
                    Password = "Password-123",
                    NormalizedEmail = "MOD@MELODYCIRCLE.PT",
                    EmailConfirmed = true,
                    Gender = Gender.Other,
                    ProfilePicture = defaultProfilePictureBytes,
                    Locality = "Portugal",
                    Connections = new List<User>(),
                    Ratings = new List<int>()
                };


                var result = await userManager.CreateAsync(user, "Password-123");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, "Mod");
                }
            }

        }
    }
}
