using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication.Internal;
using Serilog;
using System.Security.Claims;
using System.Text.Json;

namespace devSplain.Client
{
    public class RoleClaimsPrincipalFactory : AccountClaimsPrincipalFactory<RemoteUserAccount>
    {
        public RoleClaimsPrincipalFactory(IAccessTokenProviderAccessor accessor) : base(accessor)
        {
        }

        public override async ValueTask<ClaimsPrincipal> CreateUserAsync(
            RemoteUserAccount account, RemoteAuthenticationUserOptions options)
        {
            try
            {
                var user = await base.CreateUserAsync(account, options);
                if (!user.Identity.IsAuthenticated)
                {
                    return user;
                }

                var identity = (ClaimsIdentity)user.Identity;

                var roleClaims = identity.FindAll(claim => claim.Type == "roles");
                if (roleClaims == null || !roleClaims.Any())
                {
                    return user;
                }

                var rolesElem = account.AdditionalProperties["roles"];
                if (rolesElem is not JsonElement roles)
                {
                    return user;
                }

                if (roles.ValueKind == JsonValueKind.Array)
                {
                    foreach (var role in roles.EnumerateArray())
                    {
                        identity.AddClaim(new Claim(options.RoleClaim, role.GetString()));
                    }
                }
                else
                {
                    identity.AddClaim(new Claim(options.RoleClaim, roles.GetString()));
                }

                return user;
            }
            catch (Exception ex)
            {
                Log.Error("ERROR: {0}", ex);
            }
            return null;
        }
    }
}
