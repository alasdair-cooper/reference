using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Options;

namespace AlasdairCooper.Reference.Components.State;

public interface IStateView
{
    public bool TryGetCurrentState<TViewData>([NotNullWhen(true)] out TViewData currentViewData) where TViewData : StateViewData;
    
    public IOptions<StateOptions> Options { get; }

    public event EventHandler<TimeSpan> StateLoadedSuccessfully;
}