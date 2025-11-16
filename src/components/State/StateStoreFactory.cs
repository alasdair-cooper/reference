using Microsoft.Extensions.DependencyInjection;

namespace AlasdairCooper.Reference.Components.State;

public class StateStoreFactory(IServiceProvider serviceProvider)
{
    public StateStore<TParameters, T> Create<TParameters, T>(Func<TParameters, T> factory, Action<StateOptions>? configureOptions = null) =>
        CreateCore<TParameters, T>((@params, _, _) => ValueTask.FromResult(factory(@params)), configureOptions);

    public StateStore<TParameters, T> Create<TParameters, T>(
        Func<TParameters, LoadingState, T> factory,
        Action<StateOptions>? configureOptions = null) =>
        CreateCore<TParameters, T>((@params, state, _) => ValueTask.FromResult(factory(@params, state)), EnableProgress(configureOptions));

    public StateStore<TParameters, T>
        Create<TParameters, T>(Func<TParameters, ValueTask<T>> factory, Action<StateOptions>? configureOptions = null) =>
        CreateCore<TParameters, T>((@params, _, _) => factory(@params), configureOptions);

    public StateStore<TParameters, T> Create<TParameters, T>(
        Func<TParameters, CancellationToken, ValueTask<T>> factory,
        Action<StateOptions>? configureOptions = null) =>
        CreateCore<TParameters, T>((@params, _, ct) => factory(@params, ct), configureOptions);

    public StateStore<TParameters, T> Create<TParameters, T>(
        Func<TParameters, LoadingState, CancellationToken, ValueTask<T>> factory,
        Action<StateOptions>? configureOptions = null) =>
        CreateCore<TParameters, T>((@params, state, ct) => factory(@params, state, ct), EnableProgress(configureOptions));

    public StateStore<T> Create<T>(Func<T> factory, Action<StateOptions>? configureOptions = null) =>
        CreateCore<T>((_, _, _) => ValueTask.FromResult(factory()), configureOptions);

    public StateStore<T> Create<T>(Func<LoadingState, T> factory, Action<StateOptions>? configureOptions = null) =>
        CreateCore<T>((_, state, _) => ValueTask.FromResult(factory(state)), EnableProgress(configureOptions));

    public StateStore<T> Create<T>(Func<ValueTask<T>> factory, Action<StateOptions>? configureOptions = null) =>
        CreateCore<T>((_, _, _) => factory(), configureOptions);

    public StateStore<T> Create<T>(Func<CancellationToken, ValueTask<T>> factory, Action<StateOptions>? configureOptions = null) =>
        CreateCore<T>((_, _, ct) => factory(ct), configureOptions);

    public StateStore<T> Create<T>(Func<LoadingState, CancellationToken, ValueTask<T>> factory, Action<StateOptions>? configureOptions = null) =>
        CreateCore<T>((_, state, ct) => factory(state, ct), EnableProgress(configureOptions));

    public StateStore<T> Create<T>(T value, Action<StateOptions>? configureOptions = null) =>
        CreateCore<T>((_, _, _) => ValueTask.FromResult(value), configureOptions);

    private StateStore<T> CreateCore<T>(StateLoader<object, T> factory, Action<StateOptions>? configureOptions = null) =>
        ActivatorUtilities.CreateInstance<StateStore<T>>(serviceProvider, factory, configureOptions ?? (_ => { }));

    private StateStore<TParameters, T>
        CreateCore<TParameters, T>(StateLoader<TParameters, T> factory, Action<StateOptions>? configureOptions = null) =>
        ActivatorUtilities.CreateInstance<StateStore<TParameters, T>>(serviceProvider, factory, configureOptions ?? (_ => { }));

    private static Action<StateOptions> EnableProgress(Action<StateOptions>? configure) =>
        x =>
        {
            configure?.Invoke(x);
            x.IsProgressSupported = true;
        };
}