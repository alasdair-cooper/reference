using AlasdairCooper.Reference.Api.Data.Entities;
using AlasdairCooper.Reference.Shared.Common;

namespace AlasdairCooper.Reference.Api.Data.Utilities;

public sealed record PricedItem(int SkuId, Money Price);