using System.ComponentModel.DataAnnotations;

namespace AlasdairCooper.Reference.Api.Data.Entities.Stocking;

public sealed class Bin(int id, string name) : ILocatable
{
    public int Id { get; init; } = id;

    [StringLength(10)]
    public string Name { get; private set; } = name;

    public Sku Sku { get; private set; } = null!;

    public List<Stock> Stock { get; init; } = null!;

    public Rack Rack { get; init; } = null!;

    public ILocatable Parent => Rack;

    public string Key => Name;

    public void AddStock(Sku sku, params IEnumerable<Stock> stock)
    {
        Sku = sku;
        Stock.AddRange(stock);
    }

    public void TransferStockTo(Bin bin)
    {
        bin.AddStock(Sku, Stock);
        Stock.Clear();
    }
}