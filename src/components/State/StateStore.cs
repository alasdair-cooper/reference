using System.Diagnostics;
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
public partial class StateStore<TParameters, T>
{
    private readonly Func<StateOptions> _buildOptions;
    private readonly StateOptions _options;
    private readonly ILogger _logger;
    private readonly TimeProvider _timeProvider;
    private readonly StateLoader<TParameters, T> _factory;
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

        _buildOptions = buildOptions;
        _options = _buildOptions();
        _factory = factory;

        State = NewLoadingState();
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

    internal LoadingState NewLoadingState() =>
        _options.IsProgressSupported ? new LoadingState(_timeProvider.GetUtcNow(), x => State = x, 0) : new LoadingState(_timeProvider.GetUtcNow());

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

            var loadingState = State as LoadingState ?? NewLoadingState();

            State = loadingState;

            _cts = new CancellationTokenSource(_options.Timeout);

            var stopwatch = new Stopwatch();

            var res =
                await Task.Run(
                    async () =>
                    {
                        stopwatch.Start();
                        var res = await _factory(parameters, loadingState, _cts.Token);
                        stopwatch.Stop();
                        return res;
                    });

            LogStateLoaded(stopwatch.Elapsed);

            State =
                res is not null
                    ? new SuccessState<T>(res, _timeProvider.GetUtcNow(), stopwatch.Elapsed)
                    : new NotFoundState(_timeProvider.GetUtcNow(), stopwatch.Elapsed);
        }
        catch (OperationCanceledException ex)
        {
            State = new TimedOutState(ex, _options.Timeout);
        }
        catch (Exception ex)
        {
            State = new ErrorState(ex);
            LogErrorLoadingState(ex);
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

    [LoggerMessage(LogLevel.Trace, "State loaded in {Elapsed}")]
    partial void LogStateLoaded(TimeSpan elapsed);

    [LoggerMessage(LogLevel.Error, "An error occurred while loading state.")]
    partial void LogErrorLoadingState(Exception ex);
}