using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AlasdairCooper.Reference.Components.State;

public class StateStoreFactory(IOptions<StateOptions> options, TimeProvider timeProvider, ILoggerFactory loggerFactory)
{
    public StateStore<TParameters, T> Create<TParameters, T>(Func<TParameters, T> factory, Action<StateOptions>? configureOptions = null) =>
        new(loggerFactory.CreateLogger(nameof(StateStore<>)), timeProvider, (@params, _) => ValueTask.FromResult(factory(@params)), GetOptions(configureOptions));

    public StateStore<TParameters, T> Create<TParameters, T>(
        Func<TParameters, LoadingState, T> factory,
        Action<StateOptions>? configureOptions = null) =>
        new(loggerFactory.CreateLogger(nameof(StateStore<>)), timeProvider, (@params, state, _) => ValueTask.FromResult(factory(@params, state)), GetOptions(configureOptions));

    public StateStore<TParameters, T> Create<TParameters, T>(
        Func<TParameters, ValueTask<T>> factory,
        Action<StateOptions>? configureOptions = null) =>
        new(loggerFactory.CreateLogger(nameof(StateStore<>)), timeProvider, (@params, _, _) => factory(@params), GetOptions(configureOptions));

    public StateStore<TParameters, T> Create<TParameters, T>(
        Func<TParameters, CancellationToken, ValueTask<T>> factory,
        Action<StateOptions>? configureOptions = null) =>
        new(loggerFactory.CreateLogger(nameof(StateStore<>)), timeProvider, factory, GetOptions(configureOptions));

    public StateStore<TParameters, T> Create<TParameters, T>(
        Func<TParameters, LoadingState, CancellationToken, ValueTask<T>> factory,
        Action<StateOptions>? configureOptions = null) =>
        new(loggerFactory.CreateLogger(nameof(StateStore<>)), timeProvider, factory, GetOptions(configureOptions));

    public StateStore<T> Create<T>(Func<T> factory, Action<StateOptions>? configureOptions = null) =>
        new(loggerFactory.CreateLogger(nameof(StateStore<>)), timeProvider, _ => ValueTask.FromResult(factory()), GetOptions(configureOptions));

    public StateStore<T> Create<T>(Func<LoadingState, T> factory, Action<StateOptions>? configureOptions = null) =>
        new(loggerFactory.CreateLogger(nameof(StateStore<>)), timeProvider, (state, _) => ValueTask.FromResult(factory(state)), GetOptions(configureOptions));

    public StateStore<T> Create<T>(Func<ValueTask<T>> factory, Action<StateOptions>? configureOptions = null) =>
        new(loggerFactory.CreateLogger(nameof(StateStore<>)), timeProvider, (_, _) => factory(), GetOptions(configureOptions));

    public StateStore<T> Create<T>(Func<CancellationToken, ValueTask<T>> factory, Action<StateOptions>? configureOptions = null) =>
        new(loggerFactory.CreateLogger(nameof(StateStore<>)), timeProvider, factory, GetOptions(configureOptions));

    public StateStore<T> Create<T>(Func<LoadingState, CancellationToken, ValueTask<T>> factory, Action<StateOptions>? configureOptions = null) =>
        new(loggerFactory.CreateLogger(nameof(StateStore<>)), timeProvider, factory, GetOptions(configureOptions));

    public StateStore<T> Create<T>(T value, Action<StateOptions>? configureOptions = null) =>
        new(loggerFactory.CreateLogger(nameof(StateStore<>)), timeProvider, (_, _) => ValueTask.FromResult(value), GetOptions(configureOptions));

    private Func<StateOptions> GetOptions(Action<StateOptions>? configure = null) =>
        () =>
        {
            var opts = new StateOptions { Timeout = options.Value.Timeout };
            configure?.Invoke(opts);
            return opts;
        };
}