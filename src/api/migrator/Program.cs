using AlasdairCooper.Reference.Api.Data;
using AlasdairCooper.Reference.Api.Migrator;
using AlasdairCooper.Reference.Orchestration.ServiceDefaults;
using AlasdairCooper.Reference.Shared;
using AlasdairCooper.Reference.Shared.Orchestration;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddOpenTelemetry().WithTracing(tracing => tracing.AddSource(MigrationWorker.ActivitySourceName));

builder.AddServiceDefaults();
builder.AddNpgsqlDbContext<ReferenceDbContext>(AspireConstants.Resources.Database);
builder.Services.AddHostedService<MigrationWorker>();

var host = builder.Build();
host.Run();