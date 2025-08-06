using AlasdairCooper.Reference.Shared.Orchestration;

var builder = DistributedApplication.CreateBuilder(args);

var api = builder.AddProject<Projects.AlasdairCooper_Reference_Api>(AspireConstants.Resources.Api);
var migrator = builder.AddProject<Projects.AlasdairCooper_Reference_Api_Migrator>(AspireConstants.Resources.Migrator);
var generator = builder.AddProject<Projects.AlasdairCooper_Reference_Api_Generator>(AspireConstants.Resources.Generator);

var internalFrontend =
    builder.AddStandaloneBlazorWebAssemblyProject<Projects.AlasdairCooper_Reference_InternalFrontend>(AspireConstants.Resources.InternalFrontend);

var passwordParam = builder.AddParameter("db-password", true);

var dbServer = builder.AddPostgres(AspireConstants.Resources.DatabaseServer).WithPassword(passwordParam).WithDataVolume(isReadOnly: false);

var db = dbServer.AddDatabase(AspireConstants.Resources.Database);;

migrator.WithReference(db).WaitFor(db);

generator.WithReference(db).WaitFor(db).WithExplicitStart();

api.WithReference(internalFrontend).WithReference(migrator).WaitForCompletion(migrator).WithReference(db).WaitFor(db);

internalFrontend.WithReference(api).WaitFor(api);

builder.Build().Run();