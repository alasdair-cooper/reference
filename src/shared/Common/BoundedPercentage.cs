using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.Numerics;

namespace AlasdairCooper.Reference.Shared.Common;

[ComplexType]
[DebuggerDisplay("{ToDisplayString()}")]
public sealed class BoundedPercentage
{
    private BoundedPercentage()
    {
    }

    public required decimal Value { get; init; }

    public static BoundedPercentage FromValue(decimal value)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(value);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(value, 1);
        
        return new BoundedPercentage { Value = value };
    }

    public static BoundedPercentage FromPercentage<T>(T percentage) where T : INumber<T>
    {
        ArgumentOutOfRangeException.ThrowIfNegative(percentage);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(percentage, T.CreateChecked(100));
        
        return FromValue(decimal.CreateChecked(percentage) / 100m);
    }

    public string ToDisplayString() => Value.ToString("P0");
}