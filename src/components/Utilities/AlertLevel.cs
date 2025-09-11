namespace AlasdairCooper.Reference.Components.Utilities;

public enum AlertLevel
{
    Default,
    Info,
    Warning,
    Success,
    Error,
}

public static class AlertLevelExtensions
{
    public static string ToIdent(this AlertLevel level) => level.ToString().ToLower();

    public static Color ToColor(this AlertLevel level) => new VarColor($"--key-{level.ToString().ToLower()}");
}