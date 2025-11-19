using Microsoft.AspNetCore.Components;

namespace AlasdairCooper.Reference.Components.Utilities;

public record RecurseChildContentContext<T>(T Value, RenderFragment? Content);