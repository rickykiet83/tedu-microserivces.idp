using System.Security.Claims;
using IdentityModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TeduMicroservices.IDP.Common;
using TeduMicroservices.IDP.Infrastructure.Common;
using TeduMicroservices.IDP.Infrastructure.Entities;
using TeduMicroservices.IDP.Infrastructure.Repositories;

namespace TeduMicroservices.IDP.Persistence;

public static class SeedUserData
{
    public static async Task EnsureSeedDataAsync(this IServiceCollection services)
    {
        await using var serviceProvider = services.BuildServiceProvider();
        using var scope = serviceProvider
            .GetRequiredService<IServiceScopeFactory>().CreateScope();
        
        await CreateAdminUserAsync(scope, "Alice", "Smith", "Alice Smith's address",
            Guid.NewGuid().ToString(), "alice123",
            SystemConstants.Roles.Administrator, "alicesmith@example.com");
        await SeedAdminPermissions(scope);
    }

    private static async Task CreateAdminUserAsync(IServiceScope scope, string firstName, string lastName,
        string address, string id, string password, string role, string email)
    {
        var userManagement = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
        var user = await userManagement.FindByNameAsync(email);
        if (user == null)
        {
            user = new User
            {
                UserName = email,
                Email = email,
                FirstName = firstName,
                LastName = lastName,
                Address = address,
                EmailConfirmed = true,
                Id = id,
            };
            var result = await userManagement.CreateAsync(user, password);
            CheckResult(result);

            var addToRoleResult = await userManagement.AddToRoleAsync(user, role);
            CheckResult(addToRoleResult);

            result = userManagement.AddClaimsAsync(user, new Claim[]
            {
                new(SystemConstants.Claims.UserName, user.UserName),
                new(SystemConstants.Claims.FirstName, user.FirstName),
                new(SystemConstants.Claims.LastName, user.LastName),
                new(SystemConstants.Claims.Roles, role),
                new(JwtClaimTypes.Address, user.Address),
                new(JwtClaimTypes.Email, user.Email),
                new(ClaimTypes.NameIdentifier, user.Id),
            }).Result;
            CheckResult(result);
        }
    }

    private static async Task SeedAdminPermissions(IServiceScope scope)
    {
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var role = await roleManager.FindByNameAsync(SystemConstants.Roles.Administrator);
        if (role is not null)
        {
            var repositoryManager = scope.ServiceProvider.GetRequiredService<IRepositoryManager>();
            var adminPermissions = await repositoryManager.Permission.GetPermissionsByRole(role.Id);
            if (!adminPermissions.Any())
            {
                await using var teduContext = scope.ServiceProvider
                    .GetRequiredService<TeduIdentityContext>();
                var permissions = PermissionHelper.GetAllPermissions();
                foreach (var permission in permissions)
                {
                    var permissionEntity = new Permission(permission.Function, permission.Command, role.Id);
                    teduContext.Permissions.Add(permissionEntity);
                }
                await teduContext.SaveChangesAsync();
            }
        }
    }
    private static void CheckResult(IdentityResult result)
    {
        if (!result.Succeeded)
        {
            throw new Exception(result.Errors.First().Description);
        }
    }
}