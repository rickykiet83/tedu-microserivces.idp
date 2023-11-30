using System.Data;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TeduMicroservices.IDP.Infrastructure.Entities;

namespace TeduMicroservices.IDP.Persistence;

public class TeduIdentityContext : IdentityDbContext<User>
{
    public IDbConnection Connection => Database.GetDbConnection();
    public TeduIdentityContext(DbContextOptions<TeduIdentityContext> options) : base(options)
    {
    }

    public DbSet<Permission> Permissions { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(typeof(TeduIdentityContext).Assembly);
        builder.ApplyIdentityConfiguration();
    }
}