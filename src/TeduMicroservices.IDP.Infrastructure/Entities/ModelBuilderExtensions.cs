using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TeduMicroservices.IDP.Infrastructure.Common;

namespace TeduMicroservices.IDP.Infrastructure.Entities;

public static class ModelBuilderExtensions
{
    public static void ApplyIdentityConfiguration(this ModelBuilder builder)
    {
        ConfigureRole(builder.Entity<IdentityRole>());
        ConfigureUser(builder.Entity<User>());
        ConfigureRoleClaim(builder.Entity<IdentityRoleClaim<string>>());
        ConfigureUserRole(builder.Entity<IdentityUserRole<string>>());
        ConfigureUserClaim(builder.Entity<IdentityUserClaim<string>>());
        ConfigureUserLogin(builder.Entity<IdentityUserLogin<string>>());
        ConfigureUserToken(builder.Entity<IdentityUserToken<string>>());
    }

    private static void ConfigureUserToken(EntityTypeBuilder<IdentityUserToken<string>> entity)
    {
        entity.ToTable("UserTokens", SystemConstants.IdentitySchema)
            .HasKey(x => new { x.UserId });
        
        entity
            .Property(x => x.UserId)
            .IsRequired()
            .HasColumnType("varchar(50)");
    }

    private static void ConfigureUserLogin(EntityTypeBuilder<IdentityUserLogin<string>> entity)
    {
        entity.ToTable("UserLogins", SystemConstants.IdentitySchema)
            .HasKey(x => x.UserId);
        
        entity
            .Property(x => x.UserId)
            .IsRequired()
            .HasColumnType("varchar(50)");
    }

    private static void ConfigureUserClaim(EntityTypeBuilder<IdentityUserClaim<string>> entity)
    {
        entity.ToTable("UserClaims", SystemConstants.IdentitySchema)
            .HasKey(x => x.Id);
        
        entity
            .Property(x => x.UserId)
            .IsRequired()
            .HasColumnType("varchar(50)");
    }

    private static void ConfigureUserRole(EntityTypeBuilder<IdentityUserRole<string>> entity)
    {
        entity.ToTable("UserRoles", SystemConstants.IdentitySchema)
            .HasKey(x => new { x.UserId, x.RoleId });
        
        entity
            .Property(x => x.UserId)
            .IsRequired()
            .HasColumnType("varchar(50)");
        entity
            .Property(x => x.RoleId)
            .IsRequired()
            .HasColumnType("varchar(50)");
    }

    private static void ConfigureRole(EntityTypeBuilder<IdentityRole> entity)
    {
        entity.ToTable("Roles", SystemConstants.IdentitySchema)
            .HasKey(x => x.Id);
        
        entity
            .Property(x => x.Id)
            .IsRequired()
            .HasColumnType("varchar(50)");

        entity
            .Property(x => x.Name)
            .IsRequired()
            .IsUnicode()
            .HasColumnType("nvarchar(150)")
            .HasMaxLength(150);
    }

    private static void ConfigureUser(EntityTypeBuilder<User> entity)
    {
        entity.ToTable("Users", SystemConstants.IdentitySchema)
            .HasKey(x => x.Id);
        
        entity
            .Property(x => x.Id)
            .IsRequired()
            .HasColumnType("varchar(50)");

        entity.HasIndex(x => x.Email);
        entity.Property(x => x.Email)
            .IsRequired()
            .HasColumnType("varchar(255)")
            .HasMaxLength(255)
            .ValueGeneratedNever()
            ;

        entity.Property(x => x.NormalizedEmail)
            .HasColumnType("varchar(255)")
            .HasMaxLength(255)
            ;

        entity.Property(x => x.UserName)
            .IsRequired()
            .HasColumnType("varchar(255)")
            .HasMaxLength(255)
            ;

        entity.Property(x => x.NormalizedUserName)
            .HasColumnType("varchar(255)")
            .HasMaxLength(255)
            ;

        entity.Property(x => x.PhoneNumber)
            .IsUnicode(false)
            .HasColumnType("varchar(20)")
            .HasMaxLength(20);
        
        entity.Property(x => x.FirstName)
            .IsRequired()
            .HasColumnType("varchar(50)")
            .HasMaxLength(50)
            ; 
        entity.Property(x => x.LastName)
            .IsRequired()
            .HasColumnType("varchar(150)")
            .HasMaxLength(150)
            ;
    }
    
    private static void ConfigureRoleClaim(EntityTypeBuilder<IdentityRoleClaim<string>> entity)
    {
        entity.ToTable("RoleClaims", SystemConstants.IdentitySchema)
            .HasKey(x => x.Id);
        
        entity
            .Property(x => x.Id)
            .IsRequired()
            .HasColumnType("varchar(50)");
    }
}