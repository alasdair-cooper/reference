using Microsoft.AspNetCore.Authorization;

namespace AlasdairCooper.Reference.Components.State;

public class AuthorizeData(string? policy, string? roles) : IAuthorizeData
{
    public string? Policy { get; set; } = policy;

    public string? Roles { get; set; } = roles;

    public string? AuthenticationSchemes { get; set; }
}