using Microsoft.AspNetCore.Authorization;

namespace ScrummersSalesApi.Backend.Products.Api.Authentication
{
    public class JwtAuthorizationHandler : AuthorizationHandler<JwtAuthorizationRequirement>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public JwtAuthorizationHandler(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, JwtAuthorizationRequirement requirement)
        {
            var httpContext = _httpContextAccessor.HttpContext;
            var user = httpContext.User;

            // Verifica si el usuario está autenticado
            if (!user.Identity.IsAuthenticated)
            {
                context.Fail();
                return Task.CompletedTask;
            }

            // Verifica si el token contiene al menos uno de los roles especificados en el requisito
            foreach (var role in requirement.Roles)
            {
                if (user.IsInRole(role))
                {
                    context.Succeed(requirement);
                    return Task.CompletedTask;
                }
            }

            return Task.CompletedTask;
        }
    }
}