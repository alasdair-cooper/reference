using System.ComponentModel;
using Microsoft.AspNetCore.Components;

namespace AlasdairCooper.Reference.Components.Dialog;

public class DialogComponentBase<TResult> : ComponentBase
{
    [Parameter]
    [EditorRequired]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public required Action<TResult> OnSubmit { get; set; }

    [Parameter]
    [EditorRequired]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public required Action OnCancel { get; set; }

    [Parameter]
    [EditorRequired]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public required Func<Task> OnLoad { get; set; }

    [Parameter]
    [EditorRequired]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public Action<object>? ConfigureParams { get; set; }

    public override async Task SetParametersAsync(ParameterView parameters)
    {
        await base.SetParametersAsync(parameters);
        ConfigureParams?.Invoke(this);
    }

    protected void Submit(TResult result) => OnSubmit(result);

    protected void Cancel() => OnCancel();

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await OnLoad();
        }
    }
}

public class DialogComponentBase : DialogComponentBase<object>
{
    protected void Close() => Submit(new object());
}