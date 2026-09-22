using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;
using Reor.Software.Auth;
using System.Reflection;
using Xunit;

namespace Reor.Software.Auth.Tests;

public class OpenApiSecurityExtensionsTests
{
    [Fact]
    public async Task UseReorSecurityTransformer_AddsOAuthSecurityToDocument()
    {
        var options = new OpenApiOptions();
        var result = options.UseReorSecurityTransformer(["wealth.api"]);
        var document = new OpenApiDocument();
        var transformers = typeof(OpenApiOptions)
            .GetField("DocumentTransformers", BindingFlags.Instance | BindingFlags.NonPublic)!
            .GetValue(options) as List<IOpenApiDocumentTransformer>;

        Assert.Same(options, result);
        Assert.NotNull(transformers);
        Assert.Single(transformers);

        await transformers[0].TransformAsync(document, null!, CancellationToken.None);

        var scheme = Assert.IsType<OpenApiSecurityScheme>(document.Components!.SecuritySchemes!["oauth2"]);
        var flow = scheme.Flows!.AuthorizationCode!;
        Assert.Equal(new Uri("https://id.reor.software/connect/authorize"), flow.AuthorizationUrl);
        Assert.Equal(new Uri("https://id.reor.software/connect/token"), flow.TokenUrl);
        Assert.Equal("required", flow.Scopes!["wealth.api"]);
        Assert.Contains("openid", flow.Scopes.Keys);
        Assert.Contains("profile", flow.Scopes.Keys);
        Assert.Contains("email", flow.Scopes.Keys);
        Assert.Single(document.Security!);
    }
}