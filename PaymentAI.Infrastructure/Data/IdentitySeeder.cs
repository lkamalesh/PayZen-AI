using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using PaymentAI.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentAI.Infrastructure.Data
{
    public static  class IdentitySeeder
    {
        public static async Task SeedAsync(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration config)
        {
            var roles = new[] { "Merchant", "Analyst" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            var jwt = config.GetSection("Credentials");

            var analyst = jwt.GetSection("Analyst");

            string analystEmail = analyst["Email"]!;
            string analystPassword = analyst["Password"]!;

            if (await userManager.FindByEmailAsync(analystEmail) == null)
            {
                var analystUser = new ApplicationUser
                {
                    Email = analystEmail,
                    UserName = analystEmail,
                    FullName = "System Analyst",
                    Country = "N/A",
                    ApiKey = Guid.NewGuid().ToString(),
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };

                var result = await userManager.CreateAsync(analystUser, analystPassword);

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(analystUser, "Analyst");
                }
            }
        }
    }
}
