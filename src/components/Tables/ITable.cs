namespace AlasdairCooper.Reference.Components.Tables;

public interface ITable<T>
{
    public void RegisterColumn(Column<T> column);
}