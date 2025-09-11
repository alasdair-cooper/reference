namespace AlasdairCooper.Reference.Components.Utilities;

public abstract record Color
{
    public abstract string ToValidCssColor();

    public static readonly Color Default = new VarColor("");

    public static implicit operator Color(AlertLevel level) => level.ToColor();
}

public record VarColor(string VariableName) : Color
{
    public override string ToValidCssColor() => $"var({VariableName})";
    
}

public record RgbaColor(int Red, int Green, int Blue, double Alpha) : Color
{
    public override string ToValidCssColor() => $"rgba({Red}, {Green}, {Blue}, {Alpha})";
}

public record HslaColor(double Hue, double Saturation, double Lightness, double Alpha) : Color
{
    public override string ToValidCssColor() => $"hsla({Hue}, {Saturation * 100}%, {Lightness * 100}%, {Alpha})";
}