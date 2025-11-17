using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authorization;

namespace AlasdairCooper.Reference.Components.State;

public abstract record StateViewData(
    DateTimeOffset RenderedAt,
    [property: JsonIgnore]
    Action Reload)
{
    public static StateViewData FromState<T>(State state, DateTimeOffset now, Action reload) =>
        state switch
        {
            SuccessState<T> successState => new SuccessStateViewData<T>(
                successState.Value,
                successState.LoadedAt,
                successState.LoadedIn,
                now,
                reload),
            ErrorState errorState => new ErrorStateViewData(errorState.Exception, now, reload),
            LoadingState loadingState => LoadingStateViewData.FromState(loadingState, now, reload),
            NotFoundState notFoundState => new NotFoundStateViewData(notFoundState.LoadedAt, notFoundState.LoadedIn, now, reload),
            TimedOutState timedOutState => new TimedOutStateViewData(timedOutState.Exception, timedOutState.Timeout, now, reload),
            _ => throw new UnreachableException()
        };
}

public sealed record SuccessStateViewData<T>(
    [property: MemberNotNull]
    T Value,
    DateTimeOffset LoadedAt,
    TimeSpan LoadedIn,
    DateTimeOffset RenderedAt,
    Action Reload) : StateViewData(RenderedAt, Reload);

public sealed record SuccessOrNotFoundStateViewData<T>(SuccessStateViewData<T>? Success, NotFoundStateViewData? NotFound);

public sealed record LoadingStateViewData(double? Progress, DateTimeOffset RenderedAt, Action Reload) : StateViewData(RenderedAt, Reload)
{
    public static LoadingStateViewData FromState(LoadingState state, DateTimeOffset now, Action reload) => new(state.Progress, now, reload);
}

public sealed record NotFoundStateViewData(DateTimeOffset LoadedAt, TimeSpan LoadedIn, DateTimeOffset RenderedAt, Action Reload) : StateViewData(
    RenderedAt,
    Reload);

public sealed record ErrorStateViewData(Exception Exception, DateTimeOffset RenderedAt, Action Reload) : StateViewData(RenderedAt, Reload);

public sealed record TimedOutStateViewData(Exception Exception, TimeSpan Timeout, DateTimeOffset RenderedAt, Action Reload) : StateViewData(
    RenderedAt,
    Reload);

public sealed record UnauthorizedStateViewData(
    ClaimsPrincipal User,
    object? Resource,
    IAuthorizeData AuthorizeData,
    DateTimeOffset RenderedAt,
    Action Reload) : StateViewData(RenderedAt, Reload);