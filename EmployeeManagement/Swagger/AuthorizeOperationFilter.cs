using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

public class AuthorizeOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var authorizeAttributes = context.MethodInfo
            .GetCustomAttributes(true)
            .OfType<AuthorizeAttribute>()
            .ToList();

        if (!authorizeAttributes.Any())
            return;

        // 🔐 Add lock icon
        operation.Security = new List<OpenApiSecurityRequirement>
        {
            new OpenApiSecurityRequirement
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
                    Array.Empty<string>()
                }
            }
        };

        // 🏷️ Add role info to description
        var roles = authorizeAttributes
            .Where(a => !string.IsNullOrEmpty(a.Roles))
            .Select(a => a.Roles)
            .Distinct();

        if (roles.Any())
        {
            operation.Description +=
                $" **Authorized Roles:** {string.Join(", ", roles)}";
        }
        else
        {
            operation.Description += " **Authentication Required**";
        }
    }
}
