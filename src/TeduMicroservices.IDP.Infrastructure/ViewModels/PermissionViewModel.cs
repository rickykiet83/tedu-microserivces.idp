using TeduMicroservices.IDP.Infrastructure.Domains;

namespace TeduMicroservices.IDP.Infrastructure.ViewModels;

public class PermissionViewModel : EntityBase<long>
{
    public string RoleId { get; set; }

    public string Function { get; set; }

    public string Command { get; set; }
}