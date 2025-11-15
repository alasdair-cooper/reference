using System.Collections.Immutable;
using System.Reflection;
using Microsoft.AspNetCore.Components;

namespace AlasdairCooper.Reference.BlazorPlayground.Utilities;

public class PageService
{
    public ImmutableArray<string> ListPageRoutes() =>
    [
        ..Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(x => x.IsAssignableTo(typeof(IComponent)))
            .SelectMany(x => x.GetCustomAttributes().OfType<RouteAttribute>().Select(routeAttribute => routeAttribute))
            .Select(x => x.Template)
            .Except(["/"])
    ];
}