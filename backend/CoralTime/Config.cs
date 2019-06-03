using CoralTime.Common.Constants;
using IdentityModel;
using IdentityServer4;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

namespace CoralTime
{
    public class Config
    {
        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResources.Email(),
                new IdentityResource(Constants.Authorization.RolesScope, new List<string> { Constants.Authorization.RoleClaimType })
            };
        }

        // Api resources.
        public static IEnumerable<ApiResource> GetApiResources()
        {
            return new List<ApiResource>
            {
                new ApiResource(Constants.Authorization.WebApiScope ) {
                    UserClaims = { JwtClaimTypes.Email, JwtClaimTypes.NickName, JwtClaimTypes.Name, JwtClaimTypes.Role, JwtClaimTypes.Id}
                }
            };
        }

        public static IEnumerable<Client> GetClients(IConfiguration config)
        {
            var accessTokenLifetime = int.Parse(config["AccessTokenLifetime"]);
            var refreshTokenLifetime = int.Parse(config["RefreshTokenLifetime"]);
            var slidingRefreshTokenLifetime = int.Parse(config["SlidingRefreshTokenLifetime"]);
            // var botSecret = config["BotSecret"];
            
            return new List<Client>
            {
                // Local authentication client
                new Client
                {
                    ClientId = Constants.Authorization.CoralTimeApp.ClientId,
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPassword, // Resource Owner Password Credential grant.
                    AllowAccessTokensViaBrowser = true,
                    RequireClientSecret = false, // This client does not need a secret to request tokens from the token endpoint.
                    
                    UpdateAccessTokenClaimsOnRefresh = true,
                    AccessTokenLifetime = accessTokenLifetime,
                    AbsoluteRefreshTokenLifetime = refreshTokenLifetime,
                    RefreshTokenExpiration = TokenExpiration.Sliding,
                    SlidingRefreshTokenLifetime = slidingRefreshTokenLifetime,
                    RefreshTokenUsage = TokenUsage.ReUse,
                    
                    AllowedScopes = {
                        IdentityServerConstants.StandardScopes.OpenId, // For UserInfo endpoint.
                        IdentityServerConstants.StandardScopes.Profile,
                        Constants.Authorization.RolesScope,
                        Constants.Authorization.WebApiScope
                    },
                    AllowOfflineAccess = true, // For refresh token.
                 },

//                // Bot authentication client
//                new Client
//                {
//                    ClientId = Constants.Authorization.CoralTimeBot.ClientId,
//                    AllowedGrantTypes = GrantTypes.ResourceOwnerPassword, // Resource Owner Password Credential grant.
//                    //AllowAccessTokensViaBrowser = true,
//                    RequireClientSecret = true, // This client needs a secret to request tokens from the token endpoint.
//                    ClientSecrets =  new List<Secret> {new Secret (value:botSecret.Sha256()) },
//                    UpdateAccessTokenClaimsOnRefresh = true,
//                    AccessTokenLifetime = accessTokenLifetime,
//                    AbsoluteRefreshTokenLifetime = refreshTokenLifetime,
//                    RefreshTokenExpiration = TokenExpiration.Sliding,
//                    SlidingRefreshTokenLifetime = slidingRefreshTokenLifetime,
//                    RefreshTokenUsage = TokenUsage.ReUse,
//                    
//                    AllowedScopes = {
//                        IdentityServerConstants.StandardScopes.OpenId, // For UserInfo endpoint.
//                        IdentityServerConstants.StandardScopes.Profile,
//                        Constants.Authorization.RolesScope,
//                        Constants.Authorization.WebApiScope
//                    },
//                    AllowOfflineAccess = true, // For refresh token.
//                },

                // Authentication client for Azure AD
                new Client
                {
                    ClientId = Constants.Authorization.CoralTimeAzure.ClientId,
                    RequireClientSecret = false, // This client does not need a secret to request tokens from the token endpoint.

                    AllowedGrantTypes = {Constants.Authorization.CoralTimeAzure.GrantType},

                    AllowedScopes =
                    {
                       IdentityServerConstants.StandardScopes.OpenId, // For UserInfo endpoint.
                       IdentityServerConstants.StandardScopes.Profile,
                       Constants.Authorization.RolesScope,
                       Constants.Authorization.WebApiScope
                    },

                    UpdateAccessTokenClaimsOnRefresh = true,
                    AccessTokenLifetime = accessTokenLifetime,
                    AbsoluteRefreshTokenLifetime = refreshTokenLifetime,
                    RefreshTokenExpiration = TokenExpiration.Sliding,
                    SlidingRefreshTokenLifetime = slidingRefreshTokenLifetime,
                    RefreshTokenUsage = TokenUsage.ReUse,
                    
                    AllowOfflineAccess = true
                }
            };
        }

        public static void CreateAuthorizationOptions(AuthorizationOptions options)
        {
            // main Policies
            options.AddPolicy(Constants.ApplicationRoleAdmin, policy =>
            {
                policy.RequireClaim(Constants.Authorization.RoleClaimType, Constants.ApplicationRoleAdmin);
            });

            options.AddPolicy(Constants.ApplicationRoleUser, policy =>
            {
                policy.RequireClaim(Constants.Authorization.RoleClaimType, Constants.ApplicationRoleUser);
            });
        }
    }
}
