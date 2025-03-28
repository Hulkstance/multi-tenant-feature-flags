using FeatureFlagManagement.Api.Authentication;
using FeatureFlagManagement.Api.FeatureFlags;
using Microsoft.FeatureManagement;

var builder = WebApplication.CreateBuilder(args);

builder.AddFeatureFlags();

builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
});

builder.Services.AddAuthenticationServices()
    .AddAuthorization();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi("/openapi/v1/openapi.json");
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1/openapi.json", "Feature Flag Management API");
        options.RoutePrefix = "";
    });
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/feature/tenant", async (IFeatureManager featureManager) =>
{
    if (await featureManager.IsEnabledAsync(FeatureFlags.LoanFacility))
    {
        return Results.Ok("Feature flag is enabled for this tenant.");
    }
    return Results.StatusCode(StatusCodes.Status403Forbidden);
})
    .RequireAuthorization();

app.Run();
