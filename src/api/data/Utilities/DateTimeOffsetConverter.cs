using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace AlasdairCooper.Reference.Api.Data.Utilities;

internal sealed  class DateTimeOffsetConverter : ValueConverter<DateTimeOffset, DateTimeOffset>
{
    public DateTimeOffsetConverter()
        : base(
            d => d.ToUniversalTime(),
            d => d.ToUniversalTime())
    {
    }
}