using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace AlasdairCooper.Reference.Components.State;

public abstract record State(StateContext Context);

public sealed record SuccessState<T>(
    [property: MemberNotNull]
    T Value, StateContext Context) : State(Context);

public sealed record SuccessOrNotFoundState<T>(T? Value, StateContext Context);

public record LoadingState : State
{
    private readonly Action? _onReport;

    public LoadingState(StateContext context, Action? onReport = null, double? progress = null) : base(context)
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

    public static LoadingState Indeterminate(Action onReport, StateContext context) => new(context, onReport);

    public static LoadingState Zero(Action onReport, StateContext context) => new(context, onReport, 0);
}

public sealed record NotFoundState(StateContext Context) : State(Context);

public sealed record ErrorState(Exception Exception, StateContext Context) : State(Context);

public sealed record TimedOutState(Exception Exception, TimeSpan Timeout, StateContext Context) : State(Context);

public sealed record UnauthorizedState(ClaimsPrincipal User, object? Resource, IAuthorizeData AuthorizeData, StateContext Context);

public class StateContext(DateTimeOffset lastLoadedAt)
{
    public DateTimeOffset LastLoadedAt { get; } = lastLoadedAt;
}