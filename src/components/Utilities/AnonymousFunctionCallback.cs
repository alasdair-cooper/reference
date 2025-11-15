using Microsoft.JSInterop;

namespace AlasdairCooper.Reference.Components.Utilities;

public class AnonymousFunctionCallback(Action callback) : IAnonymousFunctionCallback
{
    [JSInvokable(identifier: "call")]
    public void Invoke() => callback();
}