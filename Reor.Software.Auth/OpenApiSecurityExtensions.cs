using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace Reor.Software.Auth;

public static class OpenApiSecurityExtensions
{
    public static OpenApiOptions UseReorSecurityTransformer(
        this OpenApiOptions options,
        string[] scopes,
        string? authorizationUrl = null,
        string? tokenUrl = null)
    {
        ArgumentNullException.ThrowIfNull(options);
        ArgumentNullException.ThrowIfNull(scopes);

        options.AddDocumentTransformer((document, _, _) =>
        {
            document.Components ??= new OpenApiComponents();
            document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();

            var authorizationEndpoint = authorizationUrl ?? "https://id.reor.software/connect/authorize";
            var tokenEndpoint = tokenUrl ?? "https://id.reor.software/connect/token";
            var scopeDescriptions = scopes.ToDictionary(scope => scope, _ => "required");
            document.Components.SecuritySchemes.Add("oauth2", new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.OAuth2,
                Flows = new OpenApiOAuthFlows
                {
                    AuthorizationCode = new OpenApiOAuthFlow
                    {
                        AuthorizationUrl = new Uri(authorizationEndpoint),
                        TokenUrl = new Uri(tokenEndpoint),
                        Scopes = new Dictionary<string, string>(scopeDescriptions)
                        {
                            { "openid", "Access the OpenID Connect user profile" },
                            { "email", "Access the user's email address" },
                            { "profile", "Access the user's profile" }
                        }
                    }
                }
            });

            document.Security =
            [
                new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecuritySchemeReference("oauth2"),
                        [..scopes, "profile", "email", "openid"]
                    }
                }
            ];

            document.SetReferenceHostDocument();

            return Task.CompletedTask;
        });

        return options;
    }
}