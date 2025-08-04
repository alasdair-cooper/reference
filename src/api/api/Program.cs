using AlasdairCooper.Reference.Api.Data;
using AlasdairCooper.Reference.Api.Features.Discounts;
using AlasdairCooper.Reference.Api.Features.Media;
using AlasdairCooper.Reference.Api.Features.Promotions;
using AlasdairCooper.Reference.Orchestration.Shared.Common;
using AlasdairCooper.Reference.Orchestration.Shared.Orchestration;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.AddNpgsqlDbContext<ReferenceDbContext>(AspireConstants.Resources.Database);

builder.Services.AddOpenApi();

builder.Services.AddDiscounts();
builder.Services.AddMedia();
builder.Services.AddPromotions();

builder.Services.AddCors(
    x =>
        x.AddDefaultPolicy(
            x =>
                x.WithOrigins(builder.Configuration.GetServiceEndpoints(AspireConstants.Resources.InternalFrontend))
                    .AllowAnyMethod()
                    .WithHeaders("X-Requested-With")));

builder.Services.ConfigureHttpJsonOptions(x => x.SerializerOptions.Converters.Add(new MoneyJsonConverter()));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    // See https://github.com/dotnet/aspnetcore/issues/57332#issuecomment-2480939916
    app.MapScalarApiReference(x => x.Servers = []);
}

app.UseHttpsRedirection();

app.MapDiscountsEndpoints();
app.MapMediaEndpoints();
app.MapPromotionsEndpoints();

app.UseCors();
app.UseStatusCodePages();
app.UseExceptionHandler();

app.Run();