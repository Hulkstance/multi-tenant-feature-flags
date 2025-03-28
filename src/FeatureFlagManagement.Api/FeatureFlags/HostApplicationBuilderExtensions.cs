using Azure.Identity;
using FeatureFlagManagement.Api.FeatureFlags.Filters;
using Microsoft.FeatureManagement;

// ReSharper disable once CheckNamespace
namespace Microsoft.AspNetCore.Builder;

public static class HostApplicationBuilderExtensions
{
    public static IHostApplicationBuilder AddFeatureFlags(this IHostApplicationBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        var appConfigEndpoint = builder.Configuration.GetValue<string>("Azure:AppConfiguration:Endpoint");

        if (!string.IsNullOrEmpty(appConfigEndpoint))
        {
            builder.Configuration.AddAzureAppConfiguration(options =>
            {
                options.Connect(new Uri(appConfigEndpoint), new DefaultAzureCredential())
                    .Select(".appconfig.featureflag/*") // Only select feature flags and avoid KeyVault references
                    .UseFeatureFlags();
            });
        }

        builder.Services.AddFeatureManagement()
            .AddFeatureFilter<TenantFeatureFilter>();
        
        return builder;
    }
}
