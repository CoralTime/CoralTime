using CoralTime.Common.Constants;
using IdentityModel;
using IdentityServer4;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;

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
                new IdentityResource("roles", new List<string> { "role" })
            };
        }

        // Api resources.
        public static IEnumerable<ApiResource> GetApiResources()
        {
            return new List<ApiResource>
            {
                new ApiResource("WebAPI" ) {
                    UserClaims = { JwtClaimTypes.Email, JwtClaimTypes.NickName, JwtClaimTypes.Name, JwtClaimTypes.Role, JwtClaimTypes.Id}
                }
            };
        }

        public static IEnumerable<Client> GetClients(int accessTokenLifetime, int refreshTokenLifetime, int slidingRefreshTokenLifetime)
        {
            return new List<Client>
            {
                // Local authentication client
                new Client
                {
                    ClientId = "coraltimeapp",
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPassword, // Resource Owner Password Credential grant.
                    AllowAccessTokensViaBrowser = true,
                    RequireClientSecret = false, // This client does not need a secret to request tokens from the token endpoint.
                    
                    UpdateAccessTokenClaimsOnRefresh = true,
                    AccessTokenLifetime = accessTokenLifetime,
                    AbsoluteRefreshTokenLifetime = refreshTokenLifetime,
                    RefreshTokenExpiration = TokenExpiration.Sliding,
                    SlidingRefreshTokenLifetime = slidingRefreshTokenLifetime,
                    RefreshTokenUsage = TokenUsage.OneTimeOnly,
                    
                    AllowedScopes = {
                        IdentityServerConstants.StandardScopes.OpenId, // For UserInfo endpoint.
                        IdentityServerConstants.StandardScopes.Profile,
                        "roles",
                        "WebAPI"
                    },
                    AllowOfflineAccess = true, // For refresh token.
                 },

                // Authentication client for Azure AD
                new Client
                {
                    ClientId = "coraltimeazure",
                    RequireClientSecret = false, // This client does not need a secret to request tokens from the token endpoint.

                    AllowedGrantTypes = {"azureAuth"},

                    AllowedScopes =
                    {
                       IdentityServerConstants.StandardScopes.OpenId, // For UserInfo endpoint.
                       IdentityServerConstants.StandardScopes.Profile,
                       "roles",
                       "WebAPI"
                    },

                    UpdateAccessTokenClaimsOnRefresh = true,
                    AccessTokenLifetime = accessTokenLifetime,
                    AbsoluteRefreshTokenLifetime = refreshTokenLifetime,
                    RefreshTokenExpiration = TokenExpiration.Sliding,
                    SlidingRefreshTokenLifetime = slidingRefreshTokenLifetime,
                    RefreshTokenUsage = TokenUsage.OneTimeOnly,
                    
                    AllowOfflineAccess = true
                }
            };
        }

        public static void CreateAuthorizatoinOptions(AuthorizationOptions options)
        {
            // main Policies
            options.AddPolicy(Constants.ApplicationRoleAdmin, policy =>
            {
                policy.RequireClaim("role", Constants.ApplicationRoleAdmin);
            });

            options.AddPolicy(Constants.ApplicationRoleUser, policy =>
            {
                policy.RequireClaim("role", Constants.ApplicationRoleUser);
            });
        }
    }
}
