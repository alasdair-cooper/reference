using AlasdairCooper.Reference.Orchestration.Shared;
using AlasdairCooper.Reference.Orchestration.Shared.Orchestration;

var builder = DistributedApplication.CreateBuilder(args);

var api = builder.AddProject<Projects.AlasdairCooper_Reference_Api>(AspireConstants.Resources.Api);
var migrator = builder.AddProject<Projects.AlasdairCooper_Reference_Api_Migrator>(AspireConstants.Resources.Migrator);
var internalFrontend = builder.AddStandaloneBlazorWebAssemblyProject<Projects.AlasdairCooper_Reference_InternalFrontend>(AspireConstants.Resources.InternalFrontend);

var passwordParam = builder.AddParameter("db-password", true);

var dbServer = builder.AddPostgres(AspireConstants.Resources.DatabaseServer).WithPassword(passwordParam);
var db = dbServer.AddDatabase(AspireConstants.Resources.Database);

migrator.WithReference(db).WaitFor(db);

api.WithReference(internalFrontend).WithReference(migrator).WaitForCompletion(migrator).WithReference(db).WaitFor(db);

internalFrontend.WithReference(api).WaitFor(api);

builder.Build().Run();
