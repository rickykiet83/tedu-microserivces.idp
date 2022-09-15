using System.Data;
using AutoMapper;
using Dapper;
using Microsoft.AspNetCore.Identity;
using TeduMicroservices.IDP.Infrastructure.Domains;
using TeduMicroservices.IDP.Infrastructure.Entities;
using TeduMicroservices.IDP.Infrastructure.ViewModels;
using TeduMicroservices.IDP.Persistence;

namespace TeduMicroservices.IDP.Infrastructure.Repositories;

public class PermissionRepository : RepositoryBase<Permission, long>, IPermissionRepository
{
    private readonly UserManager<User> _userManager;
    private readonly IMapper _mapper;
    
    public PermissionRepository(TeduIdentityContext dbContext, IUnitOfWork unitOfWork, UserManager<User> userManager, IMapper mapper) : base(dbContext, unitOfWork)
    {
        _userManager = userManager;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<PermissionViewModel>> GetPermissionsByRole(string roleId)
    {
        var parameters = new DynamicParameters();
        parameters.Add("@roleId", roleId);
        var result = await QueryAsync<PermissionViewModel>(
            "Get_Permission_ByRoleId",
            parameters);

        return result;
    }

    public async Task<PermissionViewModel?> CreatePermission(string roleId, PermissionAddModel model)
    {
        var parameters = new DynamicParameters();
        parameters.Add("@roleId", roleId, DbType.String);
        parameters.Add("@function", model.Function, DbType.String);
        parameters.Add("@command", model.Command, DbType.String);
        parameters.Add("@newID", dbType: DbType.Int64, direction: ParameterDirection.Output);

        var result = await ExecuteAsync("Create_Permission", parameters);
        if (result <= 0) return null;
        var newId = parameters.Get<long>("@newID");
        return new PermissionViewModel
        {
            Id = newId,
            RoleId = roleId,
            Function = model.Function,
            Command = model.Command
        };
    }

    public Task DeletePermission(string roleId, string function, string command)
    {
        var parameters = new DynamicParameters();
        parameters.Add("@roleId", roleId, DbType.String);
        parameters.Add("@function", function, DbType.String);
        parameters.Add("@command", command, DbType.String);

        return ExecuteAsync("Delete_Permission", parameters);
    }

    public Task UpdatePermissionsByRoleId(string roleId, IEnumerable<PermissionAddModel> permissionCollection)
    {
        var dt = new DataTable();
        dt.Columns.Add("RoleId", typeof(string));
        dt.Columns.Add("Function", typeof(string));
        dt.Columns.Add("Command", typeof(string));
        foreach (var item in permissionCollection)
        {
            dt.Rows.Add(roleId, item.Function, item.Command);
        }
        var parameters = new DynamicParameters();
        parameters.Add("@roleId", roleId, DbType.String);
        parameters.Add("@permissions", dt.AsTableValuedParameter("dbo.Permission"));
        return ExecuteAsync("Update_Permissions_ByRole", parameters);
    }

    public async Task<IEnumerable<PermissionUserViewModel>> GetPermissionsByUser(User user)
    {
        var currentUserRoles = await _userManager.GetRolesAsync(user);
        var query = FindAll(false)
            .Where(x => currentUserRoles.Contains(x.RoleId))
            .Select(x => new Permission(x.Id, x.Function, x.Command, x.RoleId));

        var result = _mapper.Map<IEnumerable<PermissionUserViewModel>>(query);
        return result;
    }
}