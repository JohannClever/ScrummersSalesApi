using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace ScrummersSalesApi.Backend.Products.Api.Authentication
{
    public static class JwtTokenMiddleware
    {
        public static IApplicationBuilder UseJwtTokenMiddleware(this IApplicationBuilder app)
        {
            return app.Use(async (ctx, next) =>
            {
                if (ctx.User.Identity?.IsAuthenticated != true)
                {
                    var result = await ctx.AuthenticateAsync(JwtBearerDefaults.AuthenticationScheme);
                    if (result.Succeeded && result.Principal != null)
                    {
                        ctx.User = result.Principal;
                    }
                }

                await next();
            });
        }

    }

    public class Policy
    {
        public string Name { get; set; }
        public List<string> Roles { get; set; }
    }

    public class AppConfig
    {
        public List<Policy> Policies { get; set; }

        public AppConfig()
        {
            Policies = new List<Policy>();
        }
    }
}