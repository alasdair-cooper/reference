using AlasdairCooper.Reference.Api.Data;
using AlasdairCooper.Reference.Api.Features.Discounts;
using AlasdairCooper.Reference.Api.Features.Media;
using AlasdairCooper.Reference.Api.Features.Promotions;
using AlasdairCooper.Reference.Api.Features.Users;
using AlasdairCooper.Reference.Api.Utilities;
using AlasdairCooper.Reference.Orchestration.Shared.Common;
using AlasdairCooper.Reference.Orchestration.Shared.Orchestration;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.AddNpgsqlDbContext<ReferenceDbContext>(AspireConstants.Resources.Database);
builder.AddCorsForClient(AspireConstants.Resources.InternalFrontend);

builder.Services.AddOpenApi();
builder.Services.AddProblemDetails();

builder.Services.AddDiscounts();
builder.Services.AddMedia();
builder.Services.AddPromotions();
builder.Services.AddUsers();

builder.Services.ConfigureHttpJsonOptions(x => x.SerializerOptions.Converters.Add(new MoneyJsonConverter()));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApiWithScalar();
}

app.UseHttpsRedirection();

app.MapDiscountsEndpoints();
app.MapMediaEndpoints();
app.MapPromotionsEndpoints();
app.MapUsersEndpoints();

app.UseCors();
app.UseStatusCodePages();
app.UseExceptionHandler();

app.Run();