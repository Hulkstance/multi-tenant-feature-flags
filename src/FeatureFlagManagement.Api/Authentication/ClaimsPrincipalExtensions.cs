using System.Security.Claims;

namespace FeatureFlagManagement.Api.Authentication;

public static class ClaimsPrincipalExtensions
{
    public static string? GetUserId(this ClaimsPrincipal principal)
    {
        return principal.FindFirstValue(ClaimTypes.NameIdentifier);
    }

    public static string? GetTenantId(this ClaimsPrincipal principal)
    {
        return principal.FindFirstValue(CustomClaimTypes.TenantIdentifier);
    }
}
