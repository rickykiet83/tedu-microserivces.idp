using TeduMicroservices.IDP.Infrastructure.Domains;
using TeduMicroservices.IDP.Infrastructure.Entities;

namespace TeduMicroservices.IDP.Infrastructure.Repositories;

public interface IPermissionRepository : IRepositoryBase<Permission, long>
{
    Task<IEnumerable<Permission>> GetPermissionsByRole(string roleId, bool trackChanges);

    void UpdatePermissionsByRoleId(string roleId, IEnumerable<Permission> permissionCollection,
        bool trackChanges);
}