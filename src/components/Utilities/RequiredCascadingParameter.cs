using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace AlasdairCooper.Reference.Components.Utilities;

public class RequiredCascadingParameterException(string expression) : Exception($"Cascading parameter {expression} is required but was not provided.")
{
    public static void ThrowIfNull<T>([NotNull] T? value, [CallerArgumentExpression("value")] string expression = "")
    {
        if (value is null)
        {
            throw new RequiredCascadingParameterException(expression);
        }
    }
}