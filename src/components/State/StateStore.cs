using Microsoft.Extensions.Logging;

namespace AlasdairCooper.Reference.Components.State;

public class StateStore<T> : StateStore<object, T>
{
    public StateStore(ILogger logger, Func<LoadingState, CancellationToken, ValueTask<T>> factory, StateOptions options) : base(
        logger,
        (_, state, ct) => factory(state, ct),
        options) { }

    public StateStore(ILogger logger, Func<CancellationToken, ValueTask<T>> factory, StateOptions options) : base(
        logger,
        (_, ct) => factory(ct),
        options) { }

    public async Task LoadAsync() => await LoadAsync(new object());
}

public class StateStore<TParameters, T>
{
    private readonly ILogger _logger;
    private readonly Func<TParameters, LoadingState, CancellationToken, ValueTask<T>> _factory;
    private readonly bool _supportsProgress;
    private readonly StateOptions _options;
    private readonly SemaphoreSlim _lock = new(1, 1);
    private CancellationTokenSource? _cts;

    public event EventHandler<State>? StateChanged;

    public StateStore(ILogger logger, Func<TParameters, LoadingState, CancellationToken, ValueTask<T>> factory, StateOptions options)
    {
        State = LoadingState.Zero(() => StateChanged?.Invoke(this, State ?? throw new InvalidOperationException()));
        _logger = logger;
        _factory = factory;
        _supportsProgress = true;
        _options = options;
    }

    public StateStore(ILogger logger, Func<TParameters, CancellationToken, ValueTask<T>> factory, StateOptions options)
    {
        State = LoadingState.Indeterminate(() => StateChanged?.Invoke(this, State ?? throw new InvalidOperationException()));
        _logger = logger;
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
            if (_cts is not null)
            {
                try
                {
                    await _cts.CancelAsync();
                }
                catch (OperationCanceledException)
                {
                    // ignore
                }
            }
            
            await _lock.WaitAsync();

            var loadingState = State as LoadingState ?? new LoadingState(() => StateChanged?.Invoke(this, State), _supportsProgress ? 0 : null);
            State = loadingState;

            _cts = new CancellationTokenSource(_options.Timeout);

            var res = await _factory(parameters, loadingState, _cts.Token);

            State =
                res switch
                {
                    not null => new SuccessState<T>(res),
                    null => new NotFoundState()
                };
        }
        catch (OperationCanceledException ex) 
        {
            State = new TimedOutState(ex, _options.Timeout);
        }
        catch (Exception ex)
        {
            State = new ErrorState(ex);
            _logger.LogError(ex, "An error occurred while loading state.");
        }
        finally
        {
            _lock.Release();
            _cts = null;
        }
    }
}