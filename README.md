# Reor.Software.Auth

`Reor.Software.Auth` is a .NET 10 class library containing reusable ASP.NET Core
authentication, authorization, and OpenAPI security extensions for Reor internal APIs.

## Installation

```shell
dotnet add package Reor.Software.Auth
```

## Authentication

Register Reor's JWT bearer authentication with the standard ASP.NET Core service collection:

```csharp
builder.Services.AddReorApiAuthentication();
```

This configures the Reor identity authority at `https://id.reor.software/` and the
`https://cosmos.reor.software` audience.

## Authorization

Create a policy requiring an authenticated user with a particular API scope:

```csharp
builder.Services.AddReorApiAuthorization("WealthApi", "wealth.api");
```

The policy checks space-separated `scope` claims case-insensitively. By default, the
configured policy is used as the default authorization policy. Pass `PolicyMode.All` to
also use it as the fallback policy.

## OpenAPI security

Add the OAuth2 authorization-code security transformer to ASP.NET Core OpenAPI:

```csharp
builder.Services.AddOpenApi(options =>
    options.UseReorSecurityTransformer(["wealth.api"]));
```

The transformer adds Reor's OAuth2 authorization and token endpoints, includes the
standard `openid`, `email`, and `profile` scopes, and applies the security requirement
globally. Custom authorization and token URLs can be supplied when needed.

## License

This project is licensed under the MIT License. See [LICENSE](LICENSE).