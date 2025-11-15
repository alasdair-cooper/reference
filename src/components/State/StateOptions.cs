using Microsoft.AspNetCore.Components;

namespace AlasdairCooper.Reference.Components.State;

public class StateOptions
{
    public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(10);

    public RenderFragment<ErrorStateViewData>? DefaultErrorContent { get; set; } = StateDefaults.DefaultContent.Error;

    public RenderFragment<LoadingStateViewData>? DefaultLoadingContent { get; set; } = StateDefaults.DefaultContent.Loading;
    
    public RenderFragment<TimedOutStateViewData>? DefaultTimedOutContent { get; set; } = StateDefaults.DefaultContent.TimedOut;
    
    public RenderFragment<UnauthorizedStateViewData>? DefaultUnauthorizedContent { get; set; } = StateDefaults.DefaultContent.Unauthorized;
    
    public RenderFragment<NotFoundStateViewData>? DefaultNotFoundContent { get; set; } = StateDefaults.DefaultContent.NotFound;
    
    public bool IsProgressSupported { get; set; } 

    public void DisableDefaultContent()
    {
        DefaultErrorContent = null;
        DefaultLoadingContent = null;
        DefaultTimedOutContent = null;
        DefaultUnauthorizedContent = null;
        DefaultNotFoundContent = null;
    }
}