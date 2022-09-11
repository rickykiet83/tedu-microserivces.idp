using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Storage;
using TeduMicroservices.IDP.Common.Domains;
using TeduMicroservices.IDP.Entities;
using TeduMicroservices.IDP.Persistence;

namespace TeduMicroservices.IDP.Common.Repositories;

public class RepositoryManager : IRepositoryManager
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly TeduIdentityContext _dbContext;
    private readonly Lazy<IPermissionRepository> _permissionRepository;

    public RepositoryManager(TeduIdentityContext dbContext, IUnitOfWork unitOfWork, UserManager<User> userManager,
        RoleManager<IdentityRole> roleManager)
    {
        _dbContext = dbContext;
        _unitOfWork = unitOfWork;
        UserManager = userManager;
        RoleManager = roleManager;
        
        _permissionRepository = new Lazy<IPermissionRepository>(() =>
            new PermissionRepository(_dbContext, _unitOfWork));
    }

    public UserManager<User> UserManager { get; }
    public RoleManager<IdentityRole> RoleManager { get; }
    public IPermissionRepository Permission => _permissionRepository.Value;

    public Task<int> SaveAsync()
        => _unitOfWork.CommitAsync();

    public Task<IDbContextTransaction> BeginTransactionAsync()
        => _dbContext.Database.BeginTransactionAsync();

    public Task EndTransactionAsync()
        => _dbContext.Database.CommitTransactionAsync();

    public void RollbackTransaction()
        => _dbContext.Database.RollbackTransactionAsync();
}