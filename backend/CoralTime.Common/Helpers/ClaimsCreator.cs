using IdentityModel;
using System.Collections.Generic;
using System.Security.Claims;

namespace CoralTime.Common.Helpers
{
    public class ClaimsCreator
    {
        public static List<Claim> CreateUserClaims (string userName, string fullName, string email, string role, int memberId)
        {
            var claims = new List<Claim>
            {
                new Claim(type: JwtClaimTypes.Email, value: email),
                new Claim(type: JwtClaimTypes.NickName, value: fullName),
                new Claim(type: JwtClaimTypes.Name, value: userName),
                new Claim(type: JwtClaimTypes.Role, value: role), 
                new Claim(JwtClaimTypes.Id, memberId.ToString(), ClaimValueTypes.Integer)
            };
            return claims;
        }
    }
}