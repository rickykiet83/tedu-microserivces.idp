using System.Security.Claims;
using IdentityModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TeduMicroservices.IDP.Entities;

namespace TeduMicroservices.IDP.Persistence;

public class SeedUserData
{
    public static void EnsureSeedData(string connectionString)
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddDbContext<TeduIdentityContext>(options =>
            options.UseSqlServer(connectionString));

        services.AddIdentity<User, IdentityRole>(o =>
            {
                o.Password.RequireDigit = false;
                o.Password.RequiredLength = 6;
                o.Password.RequireUppercase = false;
                o.Password.RequireLowercase = false;
                o.Password.RequireNonAlphanumeric = false;
            }).AddEntityFrameworkStores<TeduIdentityContext>()
            .AddDefaultTokenProviders();

        using (var serviceProvider = services.BuildServiceProvider())
        {
            using (var scope = serviceProvider
                       .GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                CreateUser(scope, "Alice", "Smith", "Alice Smith's Wollongong",
                    Guid.NewGuid().ToString(), "alice123",
                    "Administrator", "AliceSmith@email.com");
            }
        }
    }

    private static void CreateUser(IServiceScope scope, string name, string lastName,
        string address, string id, string password, string role, string email)
    {
        var userMgr = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
        var user = userMgr.FindByNameAsync(email).Result;
        if (user == null)
        {
            user = new User
            {
                UserName = email, Email = email, FirstName = name,
                LastName = lastName, Address = address,
                Id = id
            };
            var result = userMgr.CreateAsync(user, password).Result;
            CheckResult(result);

            result = userMgr.AddToRoleAsync(user, role).Result;
            CheckResult(result);

            result = userMgr.AddClaimsAsync(user, new Claim[]
            {
                new(JwtClaimTypes.GivenName, user.FirstName),
                new(JwtClaimTypes.FamilyName, user.LastName),
                new(JwtClaimTypes.Role, role),
                new(JwtClaimTypes.Address, user.Address)
            }).Result;
            CheckResult(result);
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