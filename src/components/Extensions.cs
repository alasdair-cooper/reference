using AlasdairCooper.Reference.Components.Utilities;
using Microsoft.JSInterop;

namespace AlasdairCooper.Reference.Components;

public static class Extensions
{
    extension(IJSRuntime jsRuntime)
    {
        public async Task<IJSObjectReference> GetModule(string path) => await jsRuntime.InvokeAsync<IJSObjectReference>("import", path);
    }

    extension(DotNetObjectReference)
    {
        public static DotNetObjectReference<IAnonymousFunctionCallback> CreateCallback(Action function) =>
            DotNetObjectReference.Create<IAnonymousFunctionCallback>(new AnonymousFunctionCallback(function));

        public static DotNetObjectReference<IAnonymousFunctionCallback> CreateCallback(Func<Task> function) =>
            DotNetObjectReference.Create<IAnonymousFunctionCallback>(new AnonymousFunctionCallbackAsync(function));
    }
}