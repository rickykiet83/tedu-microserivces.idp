using Duende.IdentityServer.Models;

namespace TeduMicroservices.IDP;

public static class Config
{
    public static IEnumerable<IdentityResource> IdentityResources =>
        new IdentityResource[]
        { 
            new IdentityResources.OpenId(),
            new IdentityResources.Profile(),
        };

    public static IEnumerable<ApiScope> ApiScopes =>
        new ApiScope[]
            { };
    public static IEnumerable<ApiResource> ApiResources =>
        new ApiResource[]
            { };

    public static IEnumerable<Client> Clients =>
        new Client[] 
            { };
}