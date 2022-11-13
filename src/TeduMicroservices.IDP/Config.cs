﻿using Duende.IdentityServer;
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
            new()
            {
                Name = "roles",
                DisplayName = "User role(s)",
                UserClaims = new List<string> { "roles" }
            }
        };

    public static IEnumerable<ApiScope> ApiScopes =>
        new ApiScope[]
        {
            new("tedu_microservices_api.read", "Tedu Microservices API Read Scope"),
            new("tedu_microservices_api.write", "Tedu Microservices API Write Scope"),
        };

    public static IEnumerable<ApiResource> ApiResources =>
        new ApiResource[]
        {
            new ApiResource("tedu_microservices_api", "Tedu Microservices API")
            {
                Scopes = new List<string> { "tedu_microservices_api.read", "tedu_microservices_api.write" },
                UserClaims = new List<string> { "roles" }
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

                RedirectUris = new List<string>()
                {
                    "http://localhost:5020/swagger/oauth2-redirect.html",
                    "http://localhost:5001/swagger/oauth2-redirect.html",
                    "http://localhost:6001/swagger/oauth2-redirect.html",
                    "http://localhost:5002/swagger/oauth2-redirect.html",
                    "http://localhost:6002/swagger/oauth2-redirect.html",
                    "http://localhost:6020/swagger/oauth2-redirect.html",
                    "https://apigw-tedu.azurewebsites.net/swagger/oauth2-redirect.html",
                    "https://product-tedu.azurewebsites.net/swagger/oauth2-redirect.html"
                },
                PostLogoutRedirectUris = new List<string>()
                {
                    "http://localhost:5020/swagger/oauth2-redirect.html",
                    "http://localhost:5001/swagger/oauth2-redirect.html",
                    "http://localhost:6001/swagger/oauth2-redirect.html",
                    "http://localhost:5002/swagger/oauth2-redirect.html",
                    "http://localhost:6002/swagger/oauth2-redirect.html",
                    "http://localhost:6020/swagger/oauth2-redirect.html",
                    "https://apigw-tedu.azurewebsites.net/swagger/oauth2-redirect.html",
                    "https://product-tedu.azurewebsites.net/swagger/oauth2-redirect.html"
                },
                AllowedCorsOrigins = new List<string>()
                {
                    "http://localhost:5020",
                    "http://localhost:5001",
                    "http://localhost:6001",
                    "http://localhost:5002",
                    "http://localhost:6002",
                    "http://localhost:6020",
                    "https://apigw-tedu.azurewebsites.net",
                    "https://product-tedu.azurewebsites.net"
                },
                AllowedScopes =
                {
                    IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.Profile,
                    IdentityServerConstants.StandardScopes.Email,
                    "tedu_microservices_api.read",
                    "tedu_microservices_api.write",
                    "tedu_microservices_api"
                }
            },
            new()
            {
                ClientName = "Tedu Microservices Postman Client",
                ClientId = "tedu_microservices_postman",
                Enabled = true,
                ClientUri = null,
                RequireClientSecret = true,
                ClientSecrets = new[]
                {
                    new Secret("SuperStrongSecret".Sha512())
                },
                AllowedGrantTypes = new[]
                {
                    GrantType.ClientCredentials,
                    GrantType.ResourceOwnerPassword
                },
                RequireConsent = false,
                AccessTokenLifetime = 60 * 60 * 2,
                AllowOfflineAccess = true,
                RedirectUris = new List<string>
                {
                    "https://www.getpostman.com/oauth2/callback"
                },
                AllowedScopes =
                {
                    IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.Profile,
                    IdentityServerConstants.StandardScopes.Email,
                    "roles",
                    "tedu_microservices_api.read",
                    "tedu_microservices_api.write",
                }
            },
        };
}