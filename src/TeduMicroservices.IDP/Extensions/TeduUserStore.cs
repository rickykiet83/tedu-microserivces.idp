using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TeduMicroservices.IDP.Infrastructure.Entities;
using TeduMicroservices.IDP.Persistence;

namespace TeduMicroservices.IDP.Extensions;

public class TeduUserStore : UserStore<User, IdentityRole, TeduIdentityContext>
{
    public TeduUserStore(TeduIdentityContext context, IdentityErrorDescriber describer = null) 
        : base(context, describer)
    {
    }
    
    // override GetRolesAsync return role ids
    public override async Task<IList<string>> GetRolesAsync(User user, CancellationToken cancellationToken = new CancellationToken())
    {
        var query = from userRole in Context.UserRoles
            join role in Context.Roles on userRole.RoleId equals role.Id
            where userRole.UserId.Equals(user.Id)
            select role.Id; // select role Id
        return await query.ToListAsync(cancellationToken);
    }
}