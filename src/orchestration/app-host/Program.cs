using AlasdairCooper.Reference.Orchestration.Shared;

var builder = DistributedApplication.CreateBuilder(args);

var api = builder.AddProject<Projects.AlasdairCooper_Reference_Api>(AspireConstants.Resources.Api);
var migrator = builder.AddProject<Projects.AlasdairCooper_Reference_Api_Migrator>(AspireConstants.Resources.Migrator);
var internalFrontend = builder.AddProject<Projects.AlasdairCooper_Reference_InternalFrontend>(AspireConstants.Resources.InternalFrontend);

var dbServer = builder.AddPostgres(AspireConstants.Resources.DatabaseServer);
var db = dbServer.AddDatabase(AspireConstants.Resources.Database);

migrator.WithReference(db).WaitFor(db);

api.WithReference(migrator).WaitForCompletion(migrator).WithReference(db).WaitFor(db);

internalFrontend.WithReference(api).WaitFor(api);

builder.Build().Run();
