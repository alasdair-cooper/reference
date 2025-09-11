using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace AlasdairCooper.Reference.Shared.Common;

public enum CurrencyType
{
    Unknown,
    Gbp,
    Usd,
}

public static class CurrencyTypeExtensions
{
    public static string ToSymbol(this CurrencyType currencyType) =>
        currencyType switch
        {
            CurrencyType.Unknown => "???",
            CurrencyType.Gbp => "£",
            CurrencyType.Usd => "$",
            _ => throw new UnreachableException()
        };
    
    public static bool TryParseSymbol(string value, [NotNullWhen(true)] out CurrencyType? currencyType)
    {
        var strippedValue = value.Trim();
        
        if (strippedValue.StartsWith('£'))
        {
            currencyType = CurrencyType.Gbp;
            return true;
        }

        if (strippedValue.StartsWith('$'))
        {
            currencyType = CurrencyType.Usd;
            return true;
        }

        currencyType = null;
        return false;
    }
    
    public static CurrencyType ParseSymbol(string value)
    {
        if(TryParseSymbol(value, out var currencyType))
        {
            return currencyType.Value;
        }
        
        throw new FormatException("Invalid currency symbol format.");
    }
}