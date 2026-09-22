using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;

namespace Reor.Software.Auth;

public static partial class AuthenticationExtensions
{
    public static IServiceCollection AddReorApiAuthentication(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);
        
        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.Authority = "https://id.reor.software/";
                options.Audience = "https://cosmos.reor.software";
            });

        return services;
    }
}