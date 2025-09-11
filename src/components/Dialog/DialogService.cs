namespace AlasdairCooper.Reference.Components.Dialog;

public class DialogService
{
    public DialogRenderer? DialogRenderer { get; internal set; }

    public async Task<DialogResult<TResult?>> ShowDialogAsync<TDialog, TResult>(Action<TDialog>? configureParams = null)
        where TDialog : DialogComponentBase<TResult>
    {
        if (DialogRenderer == null)
        {
            throw new InvalidOperationException("DialogRenderer is not set.");
        }

        return await DialogRenderer.RenderDialogAsync<TDialog, TResult>(configureParams ?? (_ => { }));
    }

    public async Task ShowDialogAsync<TDialog>(Action<TDialog>? configureParams = null) where TDialog : DialogComponentBase
    {
        if (DialogRenderer == null)
        {
            throw new InvalidOperationException("DialogRenderer is not set.");
        }

        await DialogRenderer.RenderDialogAsync<TDialog, object>(configureParams ?? (_ => { }));
    }
}

public record DialogResult<T>;

public record SuccessDialogResult<T>(T Value) : DialogResult<T>;

public record CancelledDialogResult<T> : DialogResult<T>;