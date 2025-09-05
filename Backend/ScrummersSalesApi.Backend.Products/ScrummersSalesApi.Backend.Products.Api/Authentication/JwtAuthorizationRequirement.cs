using Microsoft.AspNetCore.Authorization;

namespace ScrummersSalesApi.Backend.Products.Api.Authentication
{
    public class JwtAuthorizationRequirement : IAuthorizationRequirement
    {
        public List<string> Roles { get; }
        public JwtAuthorizationRequirement(List<string> Roles)
        {
            this.Roles = Roles;
        }
    }
}