using System.Text.Json.Serialization;
using AlasdairCooper.Reference.Api.Data;
using AlasdairCooper.Reference.Api.Features.Content;
using AlasdairCooper.Reference.Api.Features.Discounts;
using AlasdairCooper.Reference.Api.Features.Items;
using AlasdairCooper.Reference.Api.Features.Promotions;
using AlasdairCooper.Reference.Api.Features.Users;
using AlasdairCooper.Reference.Api.Utilities;
using AlasdairCooper.Reference.Shared.Common;
using AlasdairCooper.Reference.Shared.Orchestration;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.AddNpgsqlDbContext<ReferenceDbContext>(AspireConstants.Resources.Database);
builder.AddCorsForClient(AspireConstants.Resources.InternalFrontend);
builder.AddRedisOutputCache(AspireConstants.Resources.Cache);

builder.Services.AddOpenApi();
builder.Services.AddProblemDetails();

builder.Services.AddDiscounts();
builder.Services.AddMedia();
builder.Services.AddPromotions();
builder.Services.AddUsers();

builder.Services.ConfigureHttpJsonOptions(static x =>
{
    x.SerializerOptions.Converters.Add(new MoneyJsonConverter());
    x.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApiWithScalar();
}

app.UseHttpsRedirection();

app.MapDiscountsEndpoints();
app.MapItemsEndpoints();
app.MapContentEndpoints();
app.MapPromotionsEndpoints();
app.MapUsersEndpoints();

app.UseCors();
app.UseExceptionHandler();
app.UseOutputCache();
app.UseStatusCodePages();

app.Run();