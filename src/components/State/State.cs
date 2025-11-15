using System.Diagnostics.CodeAnalysis;

namespace AlasdairCooper.Reference.Components.State;

public abstract record State;

public sealed record SuccessState<T>(
    [property: MemberNotNull]
    T Value,
    DateTimeOffset LoadedAt,
    TimeSpan LoadedIn) : State;

public sealed record LoadingState(DateTimeOffset LoadingStartedAt, Action<LoadingState>? OnReport = null, double? Progress = null) : State
{
    public void Report(double progress)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(progress);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(progress, 1);
        
        OnReport?.Invoke(this with { Progress = progress });
    }
}

public sealed record NotFoundState(DateTimeOffset LoadedAt, TimeSpan LoadedIn) : State;

public sealed record ErrorState(Exception Exception) : State;

public sealed record TimedOutState(Exception Exception, TimeSpan Timeout) : State;