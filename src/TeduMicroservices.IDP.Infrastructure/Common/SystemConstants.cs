namespace TeduMicroservices.IDP.Infrastructure.Common;

public static class SystemConstants
{
    public const string IdentitySchema = "Identity";

    public static class Claims
    {
        public const string Roles = "roles";
        public const string Permissions = "permissions";
        public const string UserId = "id";
        public const string UserName = "userName";
        public const string FirstName = "firstName";
        public const string LastName = "lastName";
    }
    
    public static class Roles
    {
        public const string Administrator = "Administrator";
        public const string Customer = "Customer";
    }
    
    public static class Functions 
    {
        public const string Role = "ROLE";
        public const string Product = "PRODUCT";
        
        public static List<string> GetAllFunctions()
        {
            return new List<string>
            {
                Role,
                Product
            };
        }
    }
    
    public static class Permissions
    {
        public const string View = "View";
        public const string Create = "Create";
        public const string Update = "Update";
        public const string Delete = "Delete";
        
        public static List<string> GetAllCommands()
        {
            return new List<string>
            {
                View,
                Create,
                Update,
                Delete
            };
        }
    }
}