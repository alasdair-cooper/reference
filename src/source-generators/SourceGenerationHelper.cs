namespace AlasdairCooper.Reference.SourceGenerators;

public class SourceGenerationHelper
{
    public const string FontAwesomeIconsAttribute =
        // lang=csharp
        """
        namespace AlasdairCooper.Reference.SourceGenerators;

        [System.AttributeUsage(System.AttributeTargets.Class)]
        public class FontAwesomeIconsAttribute(string url) : System.Attribute;
        """;
}