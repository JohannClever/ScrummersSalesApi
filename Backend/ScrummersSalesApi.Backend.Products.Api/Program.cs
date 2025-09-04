using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using ScrummersSalesApi.Backend.Products.Api.Authentication;
using ScrummersSalesApi.Backend.Products.Infrastructure.Authorization;
using ScrummersSalesApi.Backend.Products.Infrastructure.DataAccess;
using ScrummersSalesApi.Backend.Products.Infrastructure.DataAccess.SeedData;
using ScrummersSalesApi.Backend.Products.Infrastructure.Extensions;
using ScrummersSalesApi.Backend.Products.Infrastructure.Mapper;
using ScrummersSalesApi.Backend.Products.Infrastructure.Security;
using SendGrid.Extensions.DependencyInjection;
using Serilog;

namespace ScrummersSalesApi.Backend.Products.Api
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            ConfigureServices(builder);

            var app = builder.Build();
            Configure(app);
            app.Run();
        }

        static void ConfigureServices(WebApplicationBuilder builder)
        {

            if (!File.Exists("publicKey.xml"))
                new RSAProviderKeyGenerator().GenerateKeys();

            builder.Host.UseSerilog((hostingContext, loggerConfiguration) =>
            {
                loggerConfiguration
                    .WriteTo.File(Path.Combine("logs", "log.txt"),
                            rollingInterval: RollingInterval.Day,
                            retainedFileCountLimit: 10);
            });

            builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            var config = builder.Configuration;

            var appConfig = new AppConfig();
            config.GetSection("Policies").Bind(appConfig.Policies);
            
            builder.Services.ConfigureSendgrid(config["Sendgrid:ApiKey"]);

            builder.Services
            .AddHttpContextAccessor()
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = JWTHelper.GetValidationParameters(config);
            });

            builder.Services
            .AddAuthorization(option =>
            {
                RegisterPolicies.AddPolicies(option, appConfig);
            });

            builder.Services.AddAutoMapper(typeof(MapperProfile).Assembly);
            var solutionDirectory = Path.GetDirectoryName(Path.GetDirectoryName(Directory.GetCurrentDirectory()));

            builder.Services
                   .AddPersistence(config)
                   .AddJwtServices()
                   .AddDomainServices()
                   .AddBusiness()
                   .AddHealthChecks();

            builder.Services.AddSingleton<IAuthorizationHandler, JwtAuthorizationHandler>();

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAllOrigins", builder =>
                    builder.AllowAnyOrigin()
                           .AllowAnyMethod()
                           .AllowAnyHeader());
            });

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Scrummers Sales API", Version = "v1" });

                var securityScheme = new OpenApiSecurityScheme
                {
                    Name = "JWT Authentication",
                    Description = "Enter your JWT token",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer", // Use "bearer" for JWT
                    BearerFormat = "JWT",
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                };

                c.AddSecurityDefinition("Bearer", securityScheme);

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                   {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] { }
                    }
                });
            });

        }

        static void Configure(WebApplication app)
        {
            app.UseSerilogRequestLogging();
            app.UseCors("AllowAllOrigins");
            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseJwtTokenMiddleware();
            app.UseAuthorization();
            app.MapControllers();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Tisur API V1");
            });
            app.MapHealthChecks("/health");

            // Compute solution directory (adjust if needed)
            var solutionDirectory = Path.GetDirectoryName(
                Path.GetDirectoryName(Directory.GetCurrentDirectory())
            )!;
            app.ApplyTablesFromFolder(solutionDirectory);

            // (Optional) seed after tables exist
            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<ProductsDbContext>();
                if (db.Database.IsSqlServer())
                {
                    DbSeeder.SeedData(db);
                }
            }
        }
    }

    public static class RegisterSendgrid
    {

        public static void ConfigureSendgrid(this IServiceCollection services,string apiKey)
        {
            // Agregar la configuración de SendGrid a tu proyecto (api key, etc.)
            services.AddSendGrid(options =>
            {
                options.ApiKey = apiKey; // Aquí debes ingresar tu API Key de SendGrid
            });
        }
    }
}