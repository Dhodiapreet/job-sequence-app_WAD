using JobSequenceApp.Models;
using Microsoft.AspNetCore.Identity;

namespace JobSequenceApp.Data
{
    public static class DbSeeder
    {
        public static async Task SeedRolesAndUsersAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var context = serviceProvider.GetRequiredService<ApplicationDbContext>();

            // Seed Roles
            string[] roleNames = { "Admin", "Customer", "Worker" };
            foreach (var roleName in roleNames)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            // Seed Admin User
            if (await userManager.FindByEmailAsync("admin@jobsequence.com") == null)
            {
                var adminUser = new ApplicationUser
                {
                    UserName = "admin@jobsequence.com",
                    Email = "admin@jobsequence.com",
                    FirstName = "System",
                    LastName = "Admin",
                    EmailConfirmed = true
                };
                var result = await userManager.CreateAsync(adminUser, "Admin123!");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }

            // Seed Customer User
            if (await userManager.FindByEmailAsync("customer@test.com") == null)
            {
                var customerUser = new ApplicationUser
                {
                    UserName = "customer@test.com",
                    Email = "customer@test.com",
                    FirstName = "John",
                    LastName = "Doe",
                    EmailConfirmed = true
                };
                var result = await userManager.CreateAsync(customerUser, "Customer123!");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(customerUser, "Customer");
                    
                    context.CustomerProfiles.Add(new CustomerProfile
                    {
                        ApplicationUserId = customerUser.Id,
                        Address = "123 Main St"
                    });
                    await context.SaveChangesAsync();
                }
            }

            // Seed Worker 1 (Plumber)
            if (await userManager.FindByEmailAsync("plumber@test.com") == null)
            {
                var plumberUser = new ApplicationUser
                {
                    UserName = "plumber@test.com",
                    Email = "plumber@test.com",
                    FirstName = "Pete",
                    LastName = "Plumber",
                    EmailConfirmed = true
                };
                var result = await userManager.CreateAsync(plumberUser, "Worker123!");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(plumberUser, "Worker");

                    context.WorkerProfiles.Add(new WorkerProfile
                    {
                        ApplicationUserId = plumberUser.Id,
                        IsVerified = true
                    });
                    await context.SaveChangesAsync();
                }
            }
            
            // Seed Worker 2 (Electrician)
            if (await userManager.FindByEmailAsync("electrician@test.com") == null)
            {
                var electricianUser = new ApplicationUser
                {
                    UserName = "electrician@test.com",
                    Email = "electrician@test.com",
                    FirstName = "Eddie",
                    LastName = "Electrician",
                    EmailConfirmed = true
                };
                var result = await userManager.CreateAsync(electricianUser, "Worker123!");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(electricianUser, "Worker");

                    context.WorkerProfiles.Add(new WorkerProfile
                    {
                        ApplicationUserId = electricianUser.Id,
                        IsVerified = true
                    });
                    await context.SaveChangesAsync();
                }
            }

            // Seed Worker 3 (AC)
            if (await userManager.FindByEmailAsync("ac@test.com") == null)
            {
                var acUser = new ApplicationUser
                {
                    UserName = "ac@test.com",
                    Email = "ac@test.com",
                    FirstName = "Alice",
                    LastName = "Cooler",
                    EmailConfirmed = true
                };
                var result = await userManager.CreateAsync(acUser, "Worker123!");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(acUser, "Worker");

                    context.WorkerProfiles.Add(new WorkerProfile
                    {
                        ApplicationUserId = acUser.Id,
                        IsVerified = true
                    });
                    await context.SaveChangesAsync();
                }
            }
        }
    }
}
