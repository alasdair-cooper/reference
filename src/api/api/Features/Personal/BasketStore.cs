namespace AlasdairCooper.Reference.Api.Features.Personal;

public class BasketStore(ISession session)
{
    public IEnumerable<BasketItemSnapshot> Items =>
        session.Get<List<BasketItemSnapshot>>("basket")?.OrderBy(static x => x.DisplayName).ToList() ?? [];

    public void AddItem(BasketItemSnapshot item)
    {
        var existingItems = Items.ToList();

        if (existingItems.FirstOrDefault(x => x.SkuId == item.SkuId) is { } existingItem)
        {
            existingItems.Remove(existingItem);
            existingItems.Add(existingItem with { Count = existingItem.Count + item.Count });
        }
        else
        {
            existingItems.Add(item);
        }

        session.Set("basket", existingItems);
    }

    public void TryUpdateItem(int skuId, Func<BasketItemSnapshot, BasketItemSnapshot> update)
    {
        var existingItems = Items.ToList();

        if (existingItems.FirstOrDefault(x => x.SkuId == skuId) is not { } existingItem) return;

        existingItems.Remove(existingItem);
        var updatedItem = update(existingItem);

        if (updatedItem.Count != 0)
        {
            existingItems.Add(updatedItem);
        }

        session.Set("basket", existingItems);
    }

    public void TryRemoveItem(int skuId)
    {
        var existingItems = Items.ToList();

        if (existingItems.FirstOrDefault(x => x.SkuId == skuId) is not { } existingItem) return;

        existingItems.Remove(existingItem);
        session.Set("basket", existingItems);
    }
}