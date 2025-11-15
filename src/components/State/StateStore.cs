using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AlasdairCooper.Reference.Components.State;

[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)]
public class StateStore<T> : StateStore<object, T>
{
    public StateStore(
        ILogger<StateStore<object, T>> logger,
        TimeProvider timeProvider,
        StateLoader<object, T> factory,
        Func<StateOptions> buildOptions) : base(logger, timeProvider, factory, buildOptions) { }

    public async Task LoadAsync() => await LoadAsync(new object());
}

[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)]
public class StateStore<TParameters, T>
{
    private readonly Func<StateOptions> _buildOptions;
    private readonly StateOptions _options;
    private readonly ILogger _logger;
    private readonly TimeProvider _timeProvider;
    private readonly StateLoader<TParameters, T> _factory;
    private readonly bool _supportsProgress;
    private readonly SemaphoreSlim _lock = new(1, 1);
    private CancellationTokenSource? _cts;

    public event EventHandler<State>? StateChanged;

    public StateStore(
        ILogger<StateStore<TParameters, T>> logger,
        TimeProvider timeProvider,
        StateLoader<TParameters, T> factory,
        Func<StateOptions> buildOptions)
    {
        _logger = logger;
        _timeProvider = timeProvider;

        State = LoadingState.Indeterminate(() => StateChanged?.Invoke(this, State ?? throw new InvalidOperationException()), NewContext());
        // State = LoadingState.Zero(() => StateChanged?.Invoke(this, State ?? throw new InvalidOperationException()), NewContext());
        _buildOptions = buildOptions;
        _options = _buildOptions();
        _factory = factory;
        _supportsProgress = true;
    }

    private StateContext NewContext() => new(_timeProvider.GetUtcNow());

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

            var loadingState =
                State as LoadingState ?? new LoadingState(NewContext(), () => StateChanged?.Invoke(this, State), _supportsProgress ? 0 : null);

            State = loadingState;

            _cts = new CancellationTokenSource(_options.Timeout);

            var res = await Task.Run(async () => await _factory(parameters, loadingState, _cts.Token));

            State =
                res switch
                {
                    not null => new SuccessState<T>(res, NewContext()),
                    null => new NotFoundState(NewContext())
                };
        }
        catch (OperationCanceledException ex)
        {
            State = new TimedOutState(ex, _options.Timeout, NewContext());
        }
        catch (Exception ex)
        {
            State = new ErrorState(ex, NewContext());
            _logger.LogError(ex, "An error occurred while loading state.");
        }
        finally
        {
            _lock.Release();
            _cts = null;
        }
    }

    public IOptions<StateOptions> Options(Action<StateOptions>? configureOptions)
    {
        var opts = _buildOptions();
        configureOptions?.Invoke(opts);
        return Microsoft.Extensions.Options.Options.Create(opts);
    }
}