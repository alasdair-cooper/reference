namespace AlasdairCooper.Reference.Components.State;

public class StateStore<T> : StateStore<object, T>
{
    public StateStore(Func<LoadingState, CancellationToken, ValueTask<T>> factory, StateOptions options) : base(
        (_, state, ct) => factory(state, ct),
        options) { }

    public StateStore(Func<CancellationToken, ValueTask<T>> factory, StateOptions options) : base((_, ct) => factory(ct), options) { }

    public async Task LoadAsync() => await LoadAsync(new object());
}

public class StateStore<TParameters, T>
{
    private readonly Func<TParameters, LoadingState, CancellationToken, ValueTask<T>> _factory;
    private readonly bool _supportsProgress;
    private readonly StateOptions _options;

    public event EventHandler<State>? StateChanged;

    public StateStore(Func<TParameters, LoadingState, CancellationToken, ValueTask<T>> factory, StateOptions options)
    {
        State = LoadingState.Zero(() => StateChanged?.Invoke(this, State ?? throw new InvalidOperationException()));
        _factory = factory;
        _supportsProgress = true;
        _options = options;
    }

    public StateStore(Func<TParameters, CancellationToken, ValueTask<T>> factory, StateOptions options)
    {
        State = LoadingState.Indeterminate(() => StateChanged?.Invoke(this, State ?? throw new InvalidOperationException()));
        _factory = (@params, _, ct) => factory(@params, ct);
        _supportsProgress = false;
        _options = options;
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