using Microsoft.EntityFrameworkCore;

namespace TeduMicroservices.IDP.Common.Domains;


public interface IUnitOfWork : IDisposable
{
    Task<int> CommitAsync();
}

public interface IUnitOfWork<TContext> : IUnitOfWork where TContext : DbContext
{
}