using Microsoft.AspNetCore.Components;

namespace AlasdairCooper.Reference.Components;

public class DefaultComponentBase : ComponentBase
{
    [Parameter]
    public string? Id { get; set; }
    
    [Parameter]
    public string? Class { get; set; }
    
    [Parameter]
    public string? Style { get; set; }
    
    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object?>? AdditionalAttributes { get; set; }
}