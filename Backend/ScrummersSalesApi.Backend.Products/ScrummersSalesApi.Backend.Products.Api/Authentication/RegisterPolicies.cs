using Microsoft.AspNetCore.Authorization;

namespace ScrummersSalesApi.Backend.Products.Api.Authentication
{
    public static class RegisterPolicies
    {
        internal static void AddPolicies(AuthorizationOptions option, AppConfig config)
        {
            foreach (var p in config.Policies)
            {
                option.AddPolicy(p.Name, policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.AddRequirements(new JwtAuthorizationRequirement(p.Roles));
                });
            }
        }
    }
}