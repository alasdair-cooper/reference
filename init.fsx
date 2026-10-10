#r "nuget: Fun.Build, 1.2.0"

open System
open System.IO
open System.IO.Compression
open System.Text.Json
open System.Xml.Linq
open Fun.Build

let root = __SOURCE_DIRECTORY__
let fork = Path.Combine(root, "lib", "efcore")
let artifacts = Path.Combine(root, "artifacts")
let feed = Path.Combine(fork, "artifacts", "packages", "Release", "Shipping")
let tools = Path.Combine(artifacts, "tools")

let version =
    use manifest = JsonDocument.Parse(File.ReadAllText(Path.Combine(root, ".config", "dotnet-tools.json")))
    manifest.RootElement.GetProperty("tools").GetProperty("dotnet-ef").GetProperty("version").GetString()

// Quote one process argument, including embedded quotes and trailing backslashes.
let quoteArgument (value: string) =
    let escaped = System.Text.RegularExpressions.Regex.Replace(value, """(\\*)"""", "$1$1\\\"")
    let escaped = System.Text.RegularExpressions.Regex.Replace(escaped, """(\\+)$""", "$1$1")
    "\"" + escaped + "\""

let verifyPackages () =
    for name in
        [ "Microsoft.EntityFrameworkCore"
          "Microsoft.EntityFrameworkCore.Abstractions"
          "Microsoft.EntityFrameworkCore.Relational"
          "Microsoft.EntityFrameworkCore.Design"
          "dotnet-ef" ] do
        let path = Path.Combine(feed, $"{name}.{version}.nupkg")
        use package = ZipFile.OpenRead(path)
        let nuspec =
            package.Entries
            |> Seq.find (fun entry -> entry.FullName.EndsWith(".nuspec", StringComparison.Ordinal))
        use contents = nuspec.Open()
        let metadata =
            XDocument.Load(contents).Root.Elements()
            |> Seq.find (fun element -> element.Name.LocalName = "metadata")
        let actualVersion =
            metadata.Elements()
            |> Seq.find (fun element -> element.Name.LocalName = "version")
        if actualVersion.Value <> version then
            failwith $"Unexpected package version in {path}."
        printfn "Verified %s" (Path.GetFileName(path))

pipeline "init" {
    description "Build the pinned EF Core fork, initialize the application, and build the solution."
    workingDir root

    stage "Checkout pinned EF Core" {
        run "git submodule update --init lib/efcore"
        run "git -C lib/efcore rev-parse HEAD"
    }

    stage "Build patched packages" {
        workingDir fork
        // Isolate activation so the application still uses the root global.json.
        stage "Windows" {
            whenWindows
            run (
                "powershell.exe -NoProfile -NonInteractive -Command " +
                quoteArgument (
                    "$ErrorActionPreference = 'Stop'; " +
                    "& .\\restore.cmd \"/p:RestoreConfigFile=$PWD\\NuGet.config\"; if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }; " +
                    ". .\\activate.ps1; " +
                    "& .\\build.cmd --configuration Release --pack " +
                    "\"/p:RestoreConfigFile=$PWD\\NuGet.config\" " +
                    $"/p:PackageVersion={version} /p:Version={version}; exit $LASTEXITCODE"))
        }
        stage "Unix" {
            whenNot { platformWindows }
            run (
                "bash -c " +
                quoteArgument (
                    "./restore.sh /p:RestoreConfigFile=\"$PWD/NuGet.config\" && " +
                    ". ./activate.sh && " +
                    "./build.sh --configuration Release --pack " +
                    "/p:RestoreConfigFile=\"$PWD/NuGet.config\" " +
                    $"/p:PackageVersion={version} /p:Version={version}"))
        }
        run (fun _ -> verifyPackages ())
    }

    stage "Clear stale application artifacts" {
        run (fun _ ->
            printfn "Removing generated application artifacts: %s" artifacts
            if Directory.Exists(artifacts) then
                Directory.Delete(artifacts, true))
    }

    stage "Initialize and build application" {
        workingDir root
        envVars [ "NUGET_PACKAGES", Path.Combine(artifacts, "nuget", "packages") ]

        stage "Restore solution" {
            run "dotnet restore AlasdairCooper.Reference.slnx --configfile nuget.config"
        }

        stage "Install patched EF CLI" {
            run (
                $"dotnet tool install dotnet-ef --version {version} --tool-path " +
                quoteArgument tools + " --configfile nuget.config --no-cache")
            let executable = if OperatingSystem.IsWindows() then "dotnet-ef.exe" else "dotnet-ef"
            run (quoteArgument (Path.Combine(tools, executable)) + " --version")
        }

        stage "Build solution" {
            run "dotnet build AlasdairCooper.Reference.slnx --no-restore"
        }
    }

    stage "Ready" {
        echo "Initialization and solution build complete."
    }

    runIfOnlySpecified false
}

tryPrintPipelineCommandHelp ()
