using Microsoft.AspNetCore.Components;

namespace AlasdairCooper.Reference.Components.State;

public class StateOptions
{
    public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(10);

    public RenderFragment<ErrorState> DefaultErrorContent { get; set; } = StateDefaults.DefaultContent.Error;

    public RenderFragment<LoadingState> DefaultLoadingContent { get; set; } = StateDefaults.DefaultContent.Loading;
    
    public RenderFragment<TimedOutState> DefaultTimedOutContent { get; set; } = StateDefaults.DefaultContent.TimedOut;
    
    public RenderFragment<UnauthorizedState> DefaultUnauthorizedContent { get; set; } = StateDefaults.DefaultContent.Unauthorized;
    
    public RenderFragment<NotFoundState> DefaultNotFoundContent { get; set; } = StateDefaults.DefaultContent.NotFound;
}