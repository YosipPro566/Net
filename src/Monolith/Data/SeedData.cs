using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

namespace AgriculturalWebsite.Data
{
    public static class SeedData
    {
        public static async Task Initialize(IServiceProvider serviceProvider, IConfiguration configuration)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();

            string[] roleNames = { "Admin", "Farmer", "Customer", "Agronomist" };

            foreach (var roleName in roleNames)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    var result = await roleManager.CreateAsync(new IdentityRole(roleName));
                    if (!result.Succeeded)
                    {
                        throw new InvalidOperationException(
                            $"Could not create role '{roleName}': {string.Join(", ", result.Errors.Select(error => error.Description))}");
                    }
                }
            }

            var seedAccounts = new[]
            {
                (Name: "Admin", Role: "Admin"),
                (Name: "Agronomist1", Role: "Agronomist"),
                (Name: "Agronomist2", Role: "Agronomist"),
                (Name: "Agronomist3", Role: "Agronomist")
            };

            foreach (var account in seedAccounts)
            {
                var section = configuration.GetSection($"SeedUsers:{account.Name}");
                var email = section["Email"];
                var password = section["Password"];

                if (email is null && password is null)
                    continue;

                if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
                {
                    throw new InvalidOperationException(
                        $"Both SeedUsers:{account.Name}:Email and SeedUsers:{account.Name}:Password must be configured.");
                }

                if (await userManager.FindByEmailAsync(email) is not null)
                    continue;

                var user = new IdentityUser
                {
                    UserName = email,
                    Email = email,
                    EmailConfirmed = true
                };

                var createResult = await userManager.CreateAsync(user, password);
                if (!createResult.Succeeded)
                {
                    throw new InvalidOperationException(
                        $"Could not create seed user '{email}': {string.Join(", ", createResult.Errors.Select(error => error.Description))}");
                }

                var roleResult = await userManager.AddToRoleAsync(user, account.Role);
                if (!roleResult.Succeeded)
                {
                    throw new InvalidOperationException(
                        $"Could not assign role '{account.Role}': {string.Join(", ", roleResult.Errors.Select(error => error.Description))}");
                }
            }
        }
    }
}
