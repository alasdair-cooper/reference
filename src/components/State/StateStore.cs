namespace AlasdairCooper.Reference.Components.State;

public class StateStore<TParameters, T>
{
    private readonly Func<TParameters, LoadingState, CancellationToken, ValueTask<T>> _factory;
    private readonly bool _supportsProgress;
    private readonly StateOptions _options;

    public event EventHandler<State>? StateChanged;

    public StateStore(Func<TParameters, LoadingState, CancellationToken, ValueTask<T>> factory, Action<StateOptions>? configureOptions)
    {
        State = LoadingState.Zero(() => StateChanged?.Invoke(this, State ?? throw new InvalidOperationException()));
        _factory = factory;
        _supportsProgress = true;
        _options = new StateOptions();
        configureOptions?.Invoke(_options);
    }

    public StateStore(Func<TParameters, CancellationToken, ValueTask<T>> factory, Action<StateOptions>? configureOptions)
    {
        State = LoadingState.Indeterminate(() => StateChanged?.Invoke(this, State ?? throw new InvalidOperationException()));
        _factory = (@params, _, ct) => factory(@params, ct);
        _supportsProgress = false;
        _options = new StateOptions();
        configureOptions?.Invoke(_options);
    }
    
    private State State
    {
        get;
        set
        {
            field = value;
            StateChanged?.Invoke(this, value);
        }
    }

    public async Task LoadAsync(TParameters parameters)
    {
        try
        {
            var loadingState = State as LoadingState ?? new LoadingState(() => StateChanged?.Invoke(this, State), _supportsProgress ? 0 : null);
            State = loadingState;

            var cts = new CancellationTokenSource(_options.Timeout);

            var res = await _factory(parameters, loadingState, cts.Token);

            State =
                res switch
                {
                    not null => new SuccessState<T>(res),
                    null => new NotFoundState()
                };
        }
        catch (TaskCanceledException ex)
        {
            State = new TimedOutState(ex, _options.Timeout);
        }
        catch (OperationCanceledException ex)
        {
            State = new TimedOutState(ex, _options.Timeout);
        }
        catch (Exception ex)
        {
            State = new ErrorState(ex);
        }
    }
}

public static class StateStore
{
    public static StateStore<TParameters, T>
        FromFactory<TParameters, T>(Func<TParameters, T> factory, Action<StateOptions>? configureOptions = null) =>
        new((@params, _) => ValueTask.FromResult(factory(@params)), configureOptions);

    public static StateStore<TParameters, T> FromFactory<TParameters, T>(
        Func<TParameters, LoadingState, T> factory,
        Action<StateOptions>? configureOptions = null) =>
        new((@params, state, _) => ValueTask.FromResult(factory(@params, state)), configureOptions);

    public static StateStore<TParameters, T> FromFactory<TParameters, T>(
        Func<TParameters, ValueTask<T>> factory,
        Action<StateOptions>? configureOptions = null) =>
        new((@params, _, _) => factory(@params), configureOptions);

    public static StateStore<TParameters, T> FromFactory<TParameters, T>(
        Func<TParameters, CancellationToken, ValueTask<T>> factory,
        Action<StateOptions>? configureOptions = null) =>
        new(factory, configureOptions);

    public static StateStore<TParameters, T> FromFactory<TParameters, T>(
        Func<TParameters, LoadingState, CancellationToken, ValueTask<T>> factory,
        Action<StateOptions>? configureOptions = null) =>
        new(factory, configureOptions);

    public static StateStore<TParameters, T> FromValue<TParameters, T>(T value, Action<StateOptions>? configureOptions = null) =>
        new((_, _) => ValueTask.FromResult(value), configureOptions);
}