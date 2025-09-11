using System.Runtime.CompilerServices;

namespace AlasdairCooper.Reference.Shared.Bcl;

// Thanks, Stephen! https://devblogs.microsoft.com/dotnet/asynclazyt/
public class AsyncLazy<T> : Lazy<Task<T>>
{
    public AsyncLazy(Func<T> valueFactory) : base(() => Task.Factory.StartNew(valueFactory)) { }

    public AsyncLazy(Func<Task<T>> taskFactory) : base(() => Task.Factory.StartNew(taskFactory).Unwrap()) { }

    public TaskAwaiter<T> GetAwaiter() => Value.GetAwaiter();
}