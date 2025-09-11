using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace AlasdairCooper.Reference.Components.State;

public abstract record State;

public sealed record SuccessState<T>(
    [NotNull]
    T Value) : State;

public sealed record SuccessOrNotFoundState<T>(T? Value);

public record LoadingState : State
{
    private readonly Action? _onReport;

    public LoadingState(Action? onReport = null, double? progress = null)
    {
        _onReport = onReport;
        Progress = progress;
    }

    public double? Progress { get; private set; }

    public void Report(double progress)
    {
        Progress = progress;
        _onReport?.Invoke();
    }

    public static LoadingState Indeterminate(Action onReport) => new(onReport);

    public static LoadingState Zero(Action onReport) => new(onReport, 0);
}

public sealed record NotFoundState : State;

public sealed record ErrorState(Exception Exception) : State;

public sealed record TimedOutState(Exception Exception, TimeSpan Timeout) : State;

public sealed record UnauthorizedState(ClaimsPrincipal User, object? Resource, IAuthorizeData AuthorizeData);