using AlasdairCooper.Reference.Api.Data;
using AlasdairCooper.Reference.Api.Generator;
using AlasdairCooper.Reference.Orchestration.ServiceDefaults;
using AlasdairCooper.Reference.Shared.Orchestration;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddOpenTelemetry().WithTracing(tracing => tracing.AddSource(GeneratorWorker.ActivitySourceName));

builder.AddServiceDefaults();
builder.AddNpgsqlDbContext<ReferenceDbContext>(AspireConstants.Resources.Database);
builder.Services.AddHostedService<GeneratorWorker>();

var host = builder.Build();
host.Run();