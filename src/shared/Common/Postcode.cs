using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;

namespace AlasdairCooper.Reference.Shared.Common;

[ComplexType]
[DebuggerDisplay("{ToDisplayString()}")]
public sealed class Postcode
{
    private Postcode()
    {
    }

    [StringLength(4)]
    public required string Outcode { get; init; }

    [StringLength(3)]
    public required string Incode { get; init; }

    public static Postcode FromString(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);

        return value.Replace(" ", "") switch
        {
            { Length: > 7 } => throw new FormatException("Postcode cannot be longer than 7 characters."),
            { Length: < 5 } => throw new FormatException("Postcode cannot be shorter than 5 characters."),
            var x => new Postcode { Outcode = x[..^3], Incode = x[^3..] }
        };
    }

    public string ToDisplayString() => $"{Outcode} {Incode}";
}