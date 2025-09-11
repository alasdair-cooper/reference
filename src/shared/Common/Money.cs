using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;

namespace AlasdairCooper.Reference.Shared.Common;

[ComplexType]
[DebuggerDisplay("{ToDisplayString()}")]
public sealed class Money : IParsable<Money>
{
    private Money()
    {
    }

    public CurrencyType Currency { get; init; }

    public decimal Value { get; init; }

    public Money ConvertTo(CurrencyType currency, decimal exchangeRate)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(exchangeRate);

        return Currency == currency ? this : FromValue(currency, Value * exchangeRate);
    }

    public static Money operator +(Money left, Money right)
    {
        CurrencyMismatchException.ThrowIfMismatch(left, right);

        return FromValue(left.Currency, left.Value + right.Value);
    }

    public static Money operator -(Money left, Money right)
    {
        CurrencyMismatchException.ThrowIfMismatch(left, right);

        return FromValue(left.Currency, Math.Max(0, left.Value - right.Value));
    }

    public static Money operator *(Money left, decimal right) => FromValue(left.Currency, left.Value * right);

    public static Money operator *(decimal left, Money right) => right * left;

    public static Money operator /(Money left, decimal right) => FromValue(left.Currency, left.Value / right);

    public static Money operator *(Money left, BoundedPercentage right) => FromValue(left.Currency, left.Value * right.Value);

    public static Money operator *(BoundedPercentage left, Money right) => right * left;

    public static implicit operator decimal(Money money) => money.Value;

    public string ToDisplayString() => $"{Currency.ToSymbol()}{Value:N2}";

    public override string ToString() => $"{Currency.ToSymbol()}{Value}";

    public static Money FromValue<T>(CurrencyType currency, T value) where T : INumber<T> =>
        new() { Currency = currency, Value = decimal.CreateChecked(value) };

    public static Money Zero(CurrencyType currency) => new() { Currency = currency, Value = 0 };

    public static Money Parse(string s, IFormatProvider? provider = null)
    {
        var currencyType = CurrencyTypeExtensions.ParseSymbol(s);
        return FromValue(currencyType, decimal.Parse(s.Trim()[1..], provider));
    }

    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, [MaybeNullWhen(false)] out Money result)
    {
        result = null!;

        try
        {
            if (s is null)
            {
                return false;
            }

            result = Parse(s, provider);

            return true;
        }
        catch
        {
            return false;
        }
    }
}