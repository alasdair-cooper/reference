using Microsoft.AspNetCore.Components;

namespace AlasdairCooper.Reference.Components.State;

public class StateOptions
{
    /// <summary>
    /// The length of time before a loading state is considered to have timed out.
    /// </summary>
    public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(10);

    /// <summary>
    /// The default content rendered when the store throws an exception or an exception was throw while rendering some other content. 
    /// </summary>
    public RenderFragment<ErrorStateViewData>? DefaultErrorContent { get; set; } = StateDefaults.DefaultContent.Error;

    /// <summary>
    /// The default content rendered the store is loading and while being authorized
    /// </summary>
    public RenderFragment<LoadingStateViewData>? DefaultLoadingContent { get; set; } = StateDefaults.DefaultContent.Loading;
    
    /// <summary>
    /// The default content rendered when the store loading times out. 
    /// </summary>
    public RenderFragment<TimedOutStateViewData>? DefaultTimedOutContent { get; set; } = StateDefaults.DefaultContent.TimedOut;
    
    /// <summary>
    /// The default content rendered when the user is unauthorized.
    /// </summary>
    public RenderFragment<UnauthorizedStateViewData>? DefaultUnauthorizedContent { get; set; } = StateDefaults.DefaultContent.Unauthorized;
    
    /// <summary>
    /// The default content rendered when the store returns <see langword="null"/>.
    /// </summary>
    public RenderFragment<NotFoundStateViewData>? DefaultNotFoundContent { get; set; } = StateDefaults.DefaultContent.NotFound;
    
    /// <summary>
    /// This enables progress reporting while loading the store. 
    /// </summary>
    /// <remarks>Set automatically when the store supports progress reporting.</remarks>
    internal bool IsProgressSupported { get; set; } 

    /// <summary>
    /// Disables all default content so that nothing is rendered by default.
    /// </summary>
    public void DisableDefaultContent()
    {
        DefaultErrorContent = null;
        DefaultLoadingContent = null;
        DefaultTimedOutContent = null;
        DefaultUnauthorizedContent = null;
        DefaultNotFoundContent = null;
    }
}