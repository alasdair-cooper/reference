using System.Diagnostics.CodeAnalysis;

namespace AlasdairCooper.Reference.Components.State;

public interface IStateView
{
    public bool TryGetCurrentState<TViewData>([NotNullWhen(true)] out TViewData currentViewData) where TViewData : StateViewData;

    public event EventHandler<TimeSpan> StateLoadedSuccessfully;
}