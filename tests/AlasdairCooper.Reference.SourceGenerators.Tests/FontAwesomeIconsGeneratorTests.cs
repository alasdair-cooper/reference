using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AlasdairCooper.Reference.SourceGenerators.Tests;

[TestClass]
public class FontAwesomeIconsGeneratorTests
{
    private const string FontAwesomeIconManifestUrl = "https://raw.githubusercontent.com/FortAwesome/Font-Awesome/refs/heads/7.x/metadata/icons.json";

    // See https://web.archive.org/web/20251119224340/https://www.meziantou.net/testing-roslyn-incremental-source-generators.htm
    [TestMethod]
    public void SourceGeneratorProducesOutput()
    {
        const string attributeName = "AlasdairCooper.Reference.SourceGenerators.FontAwesomeIcons";
        
        var compilation =
            CSharpCompilation.Create(
                "TestProject",
                [
                    CSharpSyntaxTree.ParseText(
                        // lang=csharp
                        $$"""[{{attributeName}}("{{FontAwesomeIconManifestUrl}}")] public partial static class Icons { }""",
                        cancellationToken: TestContext.CancellationTokenSource.Token)
                ],
                Basic.Reference.Assemblies.Net100.References.All,
                new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        var generator = new FontAwesomeIconsGenerator();
        var sourceGenerator = generator.AsSourceGenerator();

        GeneratorDriver driver =
            CSharpGeneratorDriver.Create([sourceGenerator], driverOptions: new GeneratorDriverOptions(default, trackIncrementalGeneratorSteps: true));

        driver = driver.RunGenerators(compilation);

        var result = driver.GetRunResult().Results.Single();
        Assert.IsTrue(result.GeneratedSources.Any(x => x.HintName == "Icons.g.cs"));
    }

    public required TestContext TestContext { get; set; }
}