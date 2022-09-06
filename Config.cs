using Duende.IdentityServer;
using Duende.IdentityServer.Models;

namespace TeduMicroservices.IDP;

public static class Config
{
    public static IEnumerable<IdentityResource> IdentityResources =>
        new IdentityResource[]
        {
            new IdentityResources.OpenId(),
            new IdentityResources.Profile(),
            new IdentityResources.Email(),
        };

    public static IEnumerable<ApiScope> ApiScopes =>
        new ApiScope[]
        {
            new("api.tedu.microservice", "Tedu Microservices API Scope")
        };

    public static IEnumerable<ApiResource> ApiResources =>
        new ApiResource[]
        {
            new ApiResource("tedumicroservicesapi", "TeduMicroservices API")
            {
                Scopes = { "api.tedu.microservice" }
            }
        };

    public static IEnumerable<Client> Clients =>
        new Client[]
        {
            new()
            {
                ClientName = "Tedu Microservices Swagger Client",
                ClientId = "tedu_microservices_swagger",

                AllowedGrantTypes = GrantTypes.Implicit,
                AllowAccessTokensViaBrowser = true,
                RequireConsent = false,
                AccessTokenLifetime = 60 * 60 * 2,

                RedirectUris = new List<string>
                {
                    "http://localhost:5001/swagger/oauth2-redirect.html",
                },
                PostLogoutRedirectUris = new List<string>
                {
                    "http://localhost:5001/swagger/oauth2-redirect.html",
                },
                AllowedCorsOrigins = new List<string>
                {
                    "https://localhost:5001"
                },
                AllowedScopes =
                {
                    IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.Profile,
                    IdentityServerConstants.StandardScopes.Email,
                    "api.tedu.microservice",
                }
            },
        };
}