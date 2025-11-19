using System.Net.Http.Json;
using System.Text;
using System.Text.Json.Serialization;
using Humanizer;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace AlasdairCooper.Reference.SourceGenerators;

public class FontAwesomeIconsGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterPostInitializationOutput(
            x => x.AddSource("FontAwesomeIconsAttribute.g.cs", SourceText.From(SourceGenerationHelper.FontAwesomeIconsAttribute, Encoding.UTF8)));

        IncrementalValuesProvider<FontAwesomeIconGeneratorContext[]> url =
            context.SyntaxProvider.ForAttributeWithMetadataName(
                    "AlasdairCooper.Reference.SourceGenerators.FontAwesomeIconsAttribute",
                    predicate: (x, _) =>
                        x is ClassDeclarationSyntax c
                        && c.Modifiers.Any(SyntaxKind.PartialKeyword)
                        && c.Modifiers.Any(SyntaxKind.StaticKeyword)
                        && c.Modifiers.Any(SyntaxKind.PublicKeyword),
                    transform: static (ctx, _) =>
                    {
                        if (ctx.TargetSymbol is not INamedTypeSymbol namedSymbol) return [];

                        var className = namedSymbol.Name;
                        var namespaceName = namedSymbol.ContainingNamespace.Name;

                        return ctx.Attributes.Select(
                                x =>
                                    new FontAwesomeIconGeneratorContext(
                                        className,
                                        namespaceName,
                                        x.ConstructorArguments.FirstOrDefault().Value!.ToString()))
                            .ToArray();
                    })
                .WithTrackingName("Syntax");

        var client = new HttpClient();

        var icons =
            url.Select(
                (generatorContext, token) =>
                    generatorContext.Select(
                        x =>
                            new FontAwesomeIconGeneratorFetchedIconsContext(
                                x.TargetClassName,
                                x.TargetClassNamespaceName,
                                (client.GetFromJsonAsync<Dictionary<string, IconDetails>>(x.Url, cancellationToken: token).GetAwaiter().GetResult()
                                    ?? []).SelectMany(entry => entry.Value.Styles, (entry, style) => new Icon(style, entry.Key))
                                .ToArray()))).WithTrackingName("FetchIcons");

        context.RegisterSourceOutput(
            icons,
            (ctx, generatorContext) =>
            {
                foreach (var context in generatorContext)
                {
                    var iconsByStyle = context.Icons.GroupBy(x => x.Style).ToDictionary(g => g.Key, g => g.ToList());

                    StringBuilder sb = new();
                    sb.AppendLine($"namespace {context.TargetClassNamespaceName};");
                    sb.AppendLine();
                    sb.AppendLine($"public static partial class {context.TargetClassName}");
                    sb.AppendLine("{");

                    foreach (var (index, style) in iconsByStyle.Select((x, i) => (i, x)))
                    {
                        var styleName = char.IsLetter(style.Key.First()) ? style.Key.Pascalize() : $"_{style.Key}";

                        if (index != 0) sb.AppendLine();
                        
                        sb.AppendLine($"    public static class {styleName}");
                        sb.AppendLine("    {");

                        foreach (var icon in style.Value)
                        {
                            var iconName = char.IsLetter(icon.Name.First()) ? icon.Name.Pascalize() : $"_{icon.Name}";
                            sb.AppendLine($"        public const string {iconName} = \"fa-{icon.Style} fa-{icon.Name}\";");
                        }

                        sb.AppendLine("    }");
                    }

                    sb.AppendLine("}");
                    ctx.AddSource($"{context.TargetClassName}.g.cs", SourceText.From(sb.ToString(), Encoding.UTF8));
                }
            });
    }
}

[Serializable]
file record IconDetails(
    [property: JsonPropertyName("styles")]
    string[] Styles);

file record Icon(string Style, string Name);

file record FontAwesomeIconGeneratorContext(string TargetClassName, string TargetClassNamespaceName, string Url);

file record FontAwesomeIconGeneratorFetchedIconsContext(string TargetClassName, string TargetClassNamespaceName, Icon[] Icons);