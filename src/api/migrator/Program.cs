using AlasdairCooper.Reference.Api.Data;
using AlasdairCooper.Reference.Api.Migrator;
using AlasdairCooper.Reference.Orchestration.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.DependencyInjection.Extensions;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddOpenTelemetry().WithTracing(tracing => tracing.AddSource(MigrationWorker.ActivitySourceName));

// if (args.Contains("--debug-migrations"))
// {
builder.Services.AddEntityFrameworkNpgsql();
builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<IParameterBindingFactory, ComplexTypeParameterBindingFactory>());
builder.Services.AddDbContext<ReferenceDbContext>((sp, x) => x.UseNpgsql("Empty").UseInternalServiceProvider(sp.GetRequiredService<IServiceProvider>()));
builder.Services.AddHostedService<DebugWorker>();
// }
// else
// {
//     builder.AddServiceDefaults();
//     builder.AddNpgsqlDbContext<ReferenceDbContext>(AspireConstants.Resources.Database);
//     builder.Services.AddHostedService<MigrationWorker>();
// }

var host = builder.Build();
host.Run();