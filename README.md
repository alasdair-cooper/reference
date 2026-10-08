# Reference

This is a project I am using to store various tricks, best practices and larger problem solutions that I find recurrently useful.

This consists of an Aspire app hosting an ASP.NET API backend for a Blazor WASM standalone frontend and a Rust ([Leptos](https://leptos.dev/)) frontend. 

## .NET and patched Entity Framework Core

`global.json` pins SDK `11.0.100-rc.1.26425.128`. Application and test projects
target `net11.0`; the source generator remains `netstandard2.0`.

The backend requires the constructor-injection fix in the EF Core fork, ported
to the **11.0 RC1 source**, not EF Core 12 main with a changed version number.
Npgsql `11.0.0-rc.1.1` requires exactly EF Core and Relational
`11.0.0-rc.1.26425.128` (verified from its published NuGet dependency metadata).
Consequently the locally patched EF packages must use that same version.

Initialize the submodule at the parent repository's recorded commit:

```sh
git submodule update --init lib/efcore
git -C lib/efcore rev-parse HEAD
```

The required RC1 port is commit `383895d43348d24000227a3aabc159b0bf104011`
on `fix/31621-complex-constructor-injection-11-rc1`, based on RC1 commit
`c22dd77aa7f7392f997cf779f0c23e0b9aab1988`. The submodule is pinned to this
patched commit, published on that branch in the configured fork.
Do not run this against EF Core 12 main.
From `lib/efcore`, bootstrap its pinned SDK and build shipping packages:

```sh
./restore.sh /p:RestoreConfigFile="$PWD/NuGet.config"
. ./activate.sh
./build.sh --configuration Release --pack \
  /p:RestoreConfigFile="$PWD/NuGet.config" \
  /p:PackageVersion=11.0.0-rc.1.26425.128 \
  /p:Version=11.0.0-rc.1.26425.128
```

On Windows, use `restore.cmd`, `. .\activate.ps1`, and `build.cmd` with the
same configuration, packing, and MSBuild arguments, using
`/p:RestoreConfigFile="$PWD\NuGet.config"` in PowerShell.
Explicitly selecting the fork's NuGet config prevents it from inheriting the
application's package source mappings; this does not bypass the application's
local EF feed when restoring the reference solution. Verify the packages in
`lib/efcore/artifacts/packages/Release/Shipping` include Core, Abstractions,
Relational, Design and `dotnet-ef` at exactly the required version, with their
EF dependencies also at that version. The EF bootstrap SDK applies only inside
the fork; return to the reference root in a fresh shell for the application.

`nuget.config` maps all `Microsoft.EntityFrameworkCore` packages and `dotnet-ef`
exclusively to the local shipping feed. Other dependencies use NuGet.org.
Missing patched packages must fail restore rather than fall back to stock EF.
The project-specific package cache is `artifacts/nuget/packages`; never point
`NUGET_PACKAGES` at a cache containing stock EF packages of this same version.
Before rebuilding the patch, remove the reference's generated `artifacts`
directory to invalidate previously restored binaries.

From the reference root:

```sh
export NUGET_PACKAGES="$PWD/artifacts/nuget/packages"
dotnet restore AlasdairCooper.Reference.sln --configfile nuget.config
dotnet build AlasdairCooper.Reference.sln --no-restore
dotnet test tests/AlasdairCooper.Reference.SourceGenerators.Tests/AlasdairCooper.Reference.SourceGenerators.Tests.csproj --no-restore
```

On macOS, if the test runner reports that its Unix socket path is too long
inside a Delta checkout, set `TESTINGPLATFORM_PIPE_DIRECTORY=/tmp` before
running `dotnet test`.

For the patched CLI, use a project-local tool installation instead of reusing
a machine-wide tool cache (the manifest records the matching version):

```sh
dotnet tool install dotnet-ef --version 11.0.0-rc.1.26425.128 \
  --tool-path artifacts/tools --configfile nuget.config --no-cache
./artifacts/tools/dotnet-ef --version
```

Do not use `--source`, `--ignore-failed-sources`, or a different NuGet config
to bypass the local EF feed. An empty cache plus missing local EF packages is
an intentional restore failure. PostgreSQL and Aspire's container resources
are additionally required to run the complete application.
