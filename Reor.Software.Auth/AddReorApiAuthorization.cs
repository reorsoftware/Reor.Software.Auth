using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace Reor.Software.Auth;

public static partial class AuthenticationExtensions
{
    public static IServiceCollection AddReorApiAuthorization(
        this IServiceCollection services,
        string policyName,
        string apiClaim,
        PolicyMode policyMode = PolicyMode.DefaultPolicy)
    {
        ArgumentNullException.ThrowIfNull(services);
        if (string.IsNullOrWhiteSpace(policyName)) throw new ArgumentNullException(nameof(policyName));
        if (string.IsNullOrWhiteSpace(apiClaim)) throw new ArgumentNullException(nameof(apiClaim));
        if (!Enum.IsDefined(policyMode)) throw new ArgumentOutOfRangeException(nameof(policyMode));

        services.AddAuthorization(options =>
        {
            var policy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .RequireAssertion(context => context.User
                    .FindAll("scope")
                    .SelectMany(claim => claim.Value.Split(' ', StringSplitOptions.RemoveEmptyEntries))
                    .Contains(apiClaim, StringComparer.InvariantCultureIgnoreCase))
                .Build();

            options.AddPolicy(policyName, policy);

            if (policyMode.HasFlag(PolicyMode.DefaultPolicy)) options.DefaultPolicy = policy;
            if (policyMode.HasFlag(PolicyMode.FallbackPolicy)) options.FallbackPolicy = policy;
        });

        return services;
    }
}