namespace AlasdairCooper.Reference.Components.Tables;

public record TableData<T>(IEnumerable<T> Data, int TotalCount)
{
    public static TableData<T> Empty => new(new List<T>(), 0);
}