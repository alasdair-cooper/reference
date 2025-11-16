using Microsoft.Extensions.DependencyInjection;

namespace AlasdairCooper.Reference.Components.State;

public class StateStoreFactory(IServiceProvider serviceProvider)
{
    public StateStore<TParameters, T> Create<TParameters, T>(Func<TParameters, T> factory, Action<StateOptions>? configureOptions = null) =>
        CreateCore<TParameters, T>((@params, _, _) => ValueTask.FromResult(factory(@params)), configureOptions);

    public StateStore<TParameters, T> Create<TParameters, T>(
        Func<TParameters, IProgressReporter, T> factory,
        Action<StateOptions>? configureOptions = null) =>
        CreateCore<TParameters, T>((@params, reporter, _) => ValueTask.FromResult(factory(@params, reporter)), EnableProgress(configureOptions));

    public StateStore<TParameters, T>
        Create<TParameters, T>(Func<TParameters, ValueTask<T>> factory, Action<StateOptions>? configureOptions = null) =>
        CreateCore<TParameters, T>((@params, _, _) => factory(@params), configureOptions);

    public StateStore<TParameters, T> Create<TParameters, T>(
        Func<TParameters, CancellationToken, ValueTask<T>> factory,
        Action<StateOptions>? configureOptions = null) =>
        CreateCore<TParameters, T>((@params, _, ct) => factory(@params, ct), configureOptions);

    public StateStore<TParameters, T> Create<TParameters, T>(
        Func<TParameters, IProgressReporter, CancellationToken, ValueTask<T>> factory,
        Action<StateOptions>? configureOptions = null) =>
        CreateCore<TParameters, T>((@params, reporter, ct) => factory(@params, reporter, ct), EnableProgress(configureOptions));

    public StateStore<T> Create<T>(Func<T> factory, Action<StateOptions>? configureOptions = null) =>
        CreateCore<T>((_, _, _) => ValueTask.FromResult(factory()), configureOptions);

    public StateStore<T> Create<T>(Func<IProgressReporter, T> factory, Action<StateOptions>? configureOptions = null) =>
        CreateCore<T>((_, reporter, _) => ValueTask.FromResult(factory(reporter)), EnableProgress(configureOptions));

    public StateStore<T> Create<T>(Func<ValueTask<T>> factory, Action<StateOptions>? configureOptions = null) =>
        CreateCore<T>((_, _, _) => factory(), configureOptions);

    public StateStore<T> Create<T>(Func<CancellationToken, ValueTask<T>> factory, Action<StateOptions>? configureOptions = null) =>
        CreateCore<T>((_, _, ct) => factory(ct), configureOptions);

    public StateStore<T> Create<T>(Func<IProgressReporter, CancellationToken, ValueTask<T>> factory, Action<StateOptions>? configureOptions = null) =>
        CreateCore<T>((_, reporter, ct) => factory(reporter, ct), EnableProgress(configureOptions));

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