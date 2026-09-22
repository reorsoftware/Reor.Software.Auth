using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Reor.Software.Auth;
using Xunit;

namespace Reor.Software.Auth.Tests;

public class AuthenticationExtensionsTests
{
    [Fact]
    public void AddReorApiAuthentication_ConfiguresJwtBearerDefaults()
    {
        var services = new ServiceCollection();

        var result = services.AddReorApiAuthentication();
        using var provider = services.BuildServiceProvider();
        var authenticationOptions = provider.GetRequiredService<IOptions<AuthenticationOptions>>().Value;
        var jwtOptions = provider.GetRequiredService<IOptionsMonitor<JwtBearerOptions>>()
            .Get(JwtBearerDefaults.AuthenticationScheme);

        Assert.Same(services, result);
        Assert.Equal(JwtBearerDefaults.AuthenticationScheme, authenticationOptions.DefaultScheme);
        Assert.Equal("https://id.reor.software/", jwtOptions.Authority);
        Assert.Equal("https://cosmos.reor.software", jwtOptions.Audience);
    }

    [Fact]
    public async Task AddReorApiAuthorization_AllowsMatchingScopeAndRejectsOthers()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddReorApiAuthorization("WealthApi", "wealth.api");
        using var provider = services.BuildServiceProvider();
        var authorizationService = provider.GetRequiredService<IAuthorizationService>();

        var matchingUser = new ClaimsPrincipal(new ClaimsIdentity(
            [new Claim("scope", "openid wealth.api")], "Bearer"));
        var otherUser = new ClaimsPrincipal(new ClaimsIdentity(
            [new Claim("scope", "openid other.api")], "Bearer"));

        var matchingResult = await authorizationService.AuthorizeAsync(matchingUser, null, "WealthApi");
        var otherResult = await authorizationService.AuthorizeAsync(otherUser, null, "WealthApi");

        Assert.True(matchingResult.Succeeded);
        Assert.False(otherResult.Succeeded);
    }

    [Fact]
    public void AddReorApiAuthorization_UsesConfiguredPolicyAsDefault()
    {
        var services = new ServiceCollection();
        services.AddReorApiAuthorization("WealthApi", "wealth.api", PolicyMode.All);
        using var provider = services.BuildServiceProvider();
        var options = provider.GetRequiredService<IOptions<AuthorizationOptions>>().Value;

        Assert.Same(options.DefaultPolicy, options.FallbackPolicy);
        Assert.Same(options.DefaultPolicy, options.GetPolicy("WealthApi"));
    }
}