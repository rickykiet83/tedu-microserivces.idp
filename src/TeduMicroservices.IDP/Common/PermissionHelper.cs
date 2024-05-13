using TeduMicroservices.IDP.Infrastructure.Common;
using TeduMicroservices.IDP.Infrastructure.ViewModels;

namespace TeduMicroservices.IDP.Common;

public static class PermissionHelper
{
    public static string GetPermission(string functionCode, string commandCode)
        => string.Join(".", functionCode, commandCode);

    /// <summary>
    /// Get all permissions predefined in the system
    /// </summary>
    /// <returns></returns>
    public static List<PermissionAddModel> GetAllPermissions()
    {
        var permissions = new List<PermissionAddModel>();
        var functions = SystemConstants.Functions.GetAllFunctions();
        var commands = SystemConstants.Permissions.GetAllCommands();

        foreach (var function in functions)
        {
            foreach (var command in commands)
            {
                permissions.Add(new PermissionAddModel
                {
                    Function = function,
                    Command = command
                });
            }
        }

        return permissions;
    }
}