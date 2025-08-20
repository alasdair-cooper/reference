using System.ComponentModel.DataAnnotations;

namespace AlasdairCooper.Reference.Api.Data.Entities.Stocking;

public sealed class Bin(int id) : ILocatable
{
    public int Id { get; private set; } = id;

    [StringLength(10)]
    public string Name { get; private set; } = null!;

    public Sku Sku { get; init; } = null!;

    public List<Stock> Stock { get; init; } = null!;

    public Rack Rack { get; init; } = null!;

    public ILocatable Parent => Rack;

    public string Key => Name;

    public void AddStock(params IEnumerable<Stock> stock) => Stock.AddRange(stock);

    public void TransferStockTo(Bin bin)
    {
        bin.AddStock(Stock);
        Stock.Clear();
    }
}