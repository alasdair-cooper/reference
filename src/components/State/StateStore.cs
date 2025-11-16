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
        IOptionsFactory<StateOptions> optionsFactory,
        Action<StateOptions> configureOptions) : base(logger, timeProvider, factory, optionsFactory, configureOptions) { }

    public async Task LoadAsync() => await LoadAsync(new object());
}

[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)]
public partial class StateStore<TParameters, T>
{
    private readonly IOptionsFactory<StateOptions> _optionsFactory;
    private readonly Action<StateOptions>? _configureOptions;
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
        IOptionsFactory<StateOptions> optionsFactory,
        Action<StateOptions>? configureOptions)
    {
        _logger = logger;
        _timeProvider = timeProvider;
        _optionsFactory = optionsFactory;
        _configureOptions = configureOptions;
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
        CreateOptions().Value.IsProgressSupported
            ? new LoadingState(_timeProvider.GetUtcNow(), x => State = x, 0)
            : new LoadingState(_timeProvider.GetUtcNow());

    public async Task LoadAsync(TParameters parameters, Action<StateOptions>? configureOptions = null)
    {
        var options = CreateOptions(configureOptions);

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

            _cts = new CancellationTokenSource(options.Value.Timeout);

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
            State = new TimedOutState(ex, options.Value.Timeout);
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

    public IOptions<StateOptions> CreateOptions(Action<StateOptions>? configureOptions = null)
    {
        var opts = _optionsFactory.Create(Options.DefaultName);

        _configureOptions?.Invoke(opts);
        configureOptions?.Invoke(opts);

        return Options.Create(opts);
    }

    [LoggerMessage(LogLevel.Trace, "State loaded in {Elapsed}")]
    partial void LogStateLoaded(TimeSpan elapsed);

    [LoggerMessage(LogLevel.Error, "An error occurred while loading state.")]
    partial void LogErrorLoadingState(Exception ex);
}