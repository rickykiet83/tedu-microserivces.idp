using TeduMicroservices.IDP.Infrastructure.Domains;
using TeduMicroservices.IDP.Infrastructure.Entities;
using TeduMicroservices.IDP.Persistence;

namespace TeduMicroservices.IDP.Infrastructure.Repositories;

public class PermissionRepository : RepositoryBase<Permission, long>, IPermissionRepository
{
    public PermissionRepository(TeduIdentityContext dbContext, IUnitOfWork unitOfWork) : base(dbContext, unitOfWork)
    {
    }
    
    public Task<IEnumerable<Permission>> GetPermissionsByRole(string roleId, bool trackChanges = false)
    {
        throw new NotImplementedException();
    }

    public void UpdatePermissionsByRoleId(string roleId, IEnumerable<Permission> permissionCollection, bool trackChanges = false)
    {
        throw new NotImplementedException();
    }

    
}