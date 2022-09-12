namespace TeduMicroservices.IDP.Infrastructure.Domains;


public interface IUnitOfWork : IDisposable
{
    Task<int> CommitAsync();
}
