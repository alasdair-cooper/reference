namespace AlasdairCooper.Reference.Shared.Common;

internal sealed class CurrencyMismatchException(CurrencyType left, CurrencyType right) : Exception(
    $"Cannot perform operation on {left} and {right} currencies.")
{
    public static void ThrowIfMismatch(Money left, Money right)
    {
        if (left.Currency != right.Currency)
        {
            throw new CurrencyMismatchException(left.Currency, right.Currency);
        }
    }
}