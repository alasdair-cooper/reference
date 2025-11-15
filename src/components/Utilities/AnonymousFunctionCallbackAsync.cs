using Microsoft.JSInterop;

namespace AlasdairCooper.Reference.Components.Utilities;

public class AnonymousFunctionCallbackAsync(Func<Task> callback) : IAnonymousFunctionCallback
{
    [JSInvokable(identifier: "call")]
    public async Task InvokeAsync() => await callback();
}