using AlasdairCooper.Reference.Orchestration.Shared;

var builder = DistributedApplication.CreateBuilder(args);

var api = builder.AddProject<Projects.AlasdairCooper_Reference_Api>(AspireConstants.Resources.Api);
var internalFrontend = builder.AddProject<Projects.AlasdairCooper_Reference_InternalFrontend>(AspireConstants.Resources.InternalFrontend);

internalFrontend.WithReference(api).WaitFor(api);

builder.Build().Run();
