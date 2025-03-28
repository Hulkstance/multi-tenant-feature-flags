using FeatureFlagManagement.Api.Authentication;
using Microsoft.FeatureManagement;

namespace FeatureFlagManagement.Api.FeatureFlags.Filters;

[FilterAlias("Tenant")]
public class TenantFeatureFilter(IHttpContextAccessor httpContextAccessor, ILogger<TenantFeatureFilter> logger)
    : IFeatureFilter
{
    private const string ParameterKey = "AllowedTenants";
    
    public Task<bool> EvaluateAsync(FeatureFilterEvaluationContext context)
    {
        var tenantId = httpContextAccessor.HttpContext?.User.GetTenantId();
        if (tenantId is null)
        {
            logger.LogWarning("TenantId not present");
            return Task.FromResult(false);
        }
        
        var allowed = FeatureFilterUtils.ExtractAllowedValues(context.Parameters, ParameterKey);
        if (allowed.Length == 0)
        {
            logger.LogWarning("No allowed tenants configured");
            return Task.FromResult(false);
        }
        
        var match = allowed.Any(x => x.Equals(tenantId.Trim(), StringComparison.OrdinalIgnoreCase));
        logger.LogDebug("TenantId '{TenantId}' match = {Match}", tenantId, match);
        
        return Task.FromResult(match);
    }
}
