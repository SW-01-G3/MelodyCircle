using MelodyCircle.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Internal;
using Moq;
using SQLitePCL;

namespace MelodyCircleTest
{
    public class UnitTestExample : IClassFixture<ApplicationDbContextFixture>
    {
        private DbContext _context;

        public UnitTestExample(ApplicationDbContextFixture applicationDbContextFixture)
        {
            _context = applicationDbContextFixture.DbContext;

        }

        [Fact]
        public void Test1()
        {
            var x = 1;
            Assert.True(x == 1);
        }

        [Fact]
        public async Task CheckUser()
        {
            var userManager = new Mock<UserManager<User>>();

            var email = "admin1@melodycircle.pt";

            var tuser = new User
            {
                UserName = "admin1",
                Email = "admin1@melodycircle.pt",
                Name = "Admin1",
                BirthDate = new DateOnly(2010, 1, 22),
                Password = "Password-123",
                NormalizedEmail = "ADMIN1@MELODYCIRCLE.PT",
                EmailConfirmed = true,
            };

            var result = await userManager.Object.FindByEmailAsync(email); 

            //Assert
            Assert.True(result.Email == email);
        }
    }
}