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
}