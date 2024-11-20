using Microsoft.OpenApi.Models;

namespace API.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSwaggerGenWithAuth(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSwaggerGen(c =>
            {
                c.CustomSchemaIds(id => id.FullName!.Replace('+', '-'));

                c.AddSecurityDefinition("Keycloak", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.OAuth2,
                    Flows = new OpenApiOAuthFlows
                    {
                        Implicit = new OpenApiOAuthFlow
                        {
                            AuthorizationUrl = new Uri(configuration["Keycloak:AuthorizationUrl"]!),
                            Scopes = new Dictionary<string, string>
                            {
                                { "openid", "openid" },
                                { "profile", "profile" }
                            }
                        }
                    }
                });

                var securityRequirement = new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Id = "Keycloak",
                                Type = ReferenceType.SecurityScheme
                            },
                            In = ParameterLocation.Header,
                            Name = "Bearer",
                            Scheme = "Bearer"
                        },
                        []
                    }
                };

                c.AddSecurityRequirement(securityRequirement);

            });

            return services;

        }
    }
}
