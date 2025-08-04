using AlasdairCooper.Reference.Api.Data.Entities;
using AlasdairCooper.Reference.Orchestration.Shared.Common;

namespace AlasdairCooper.Reference.Api.Data.Utilities;

public sealed record PricedItem(Sku Sku, Money Price);