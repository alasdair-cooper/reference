using AlasdairCooper.Reference.Api.Data;
using AlasdairCooper.Reference.Api.Features.Content;
using AlasdairCooper.Reference.Api.Features.Discounts;
using AlasdairCooper.Reference.Api.Features.Items;
using AlasdairCooper.Reference.Api.Features.Personal;
using AlasdairCooper.Reference.Api.Features.Promotions;
using AlasdairCooper.Reference.Api.Features.Users;
using AlasdairCooper.Reference.Api.Utilities;
using AlasdairCooper.Reference.Orchestration.ServiceDefaults;
using AlasdairCooper.Reference.Shared.Orchestration;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.AddNpgsqlDbContext<ReferenceDbContext>(AspireConstants.Resources.Database);
builder.AddCorsForClient(AspireConstants.Resources.InternalFrontend);
builder.AddRedisOutputCache(AspireConstants.Resources.Cache);
builder.AddRedisDistributedCache(AspireConstants.Resources.Cache);

builder.Services.AddAuthentication(static x => x.RequireAuthenticatedSignIn = false).AddCookie(static x =>
{
    x.Cookie.SameSite = SameSiteMode.None;
    x.AccessDeniedPath = "/access-denied";
});
builder.Services.AddAuthorization();
builder.Services.AddOpenApi();
builder.Services.AddProblemDetails();
builder.Services.AddSession(static x =>
{
    x.Cookie.SameSite = SameSiteMode.None;
    x.Cookie.SecurePolicy = CookieSecurePolicy.Always;
});
builder.Services.AddSignalR();

builder.Services.ConfigureJson();

builder.Services.AddDiscounts();
builder.Services.AddMedia();
builder.Services.AddPromotions();
builder.Services.AddUsers();

var app = builder.Build();

app.UseWebSockets();
app.UseHttpsRedirection();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
app.UseExceptionHandler();
app.UseOutputCache();
app.UseStatusCodePages();
app.UseSession();

app.MapDiscountsEndpoints();
app.MapItemsEndpoints();
app.MapContentEndpoints();
app.MapPersonalEndpoints();
app.MapPromotionsEndpoints();
app.MapUsersEndpoints();

app.MapGet("/access-denied", static () => Results.Problem("Access denied.", statusCode: StatusCodes.Status403Forbidden));

if (app.Environment.IsDevelopment())
{
    app.MapOpenApiWithScalar();
}

app.MapHub<PersonalHub>("/personal");

app.Run();