using System.ComponentModel.DataAnnotations;
using System.Text;
using AlasdairCooper.Reference.Api.Data.Utilities;
using AlasdairCooper.Reference.Shared.Common;

namespace AlasdairCooper.Reference.Api.Data.Entities.Addresses;

public sealed class UkAddress(int id, string line1, string? line2, string? line3, string? townCity, string? county, Postcode postcode) : Address(id)
{
    [StringLength(100)]
    public string Line1 { get; init; } = line1;

    [StringLength(100)]
    public string? Line2 { get; init; } = line2;

    [StringLength(100)]
    public string? Line3 { get; init; } = line3;

    [StringLength(100)]
    public string? TownCity { get; init; } = townCity;

    [StringLength(100)]
    public string? County { get; init; } = county;

    public Postcode Postcode { get; init; } = postcode;

    public override string ToDisplayString()
    {
        var sb = new StringBuilder();

        sb.AppendLine(Line1);

        if (!string.IsNullOrWhiteSpace(Line2))
        {
            sb.AppendLine(Line2);
        }

        if (!string.IsNullOrWhiteSpace(Line3))
        {
            sb.AppendLine(Line3);
        }

        if (!string.IsNullOrWhiteSpace(TownCity))
        {
            sb.AppendLine(TownCity);
        }

        if (!string.IsNullOrWhiteSpace(County))
        {
            sb.AppendLine(County);
        }

        sb.AppendLine(Postcode.ToDisplayString());

        sb.AppendLine(Country.ToDisplayString());

        return sb.ToString();
    }
}