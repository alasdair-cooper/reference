namespace AlasdairCooper.Reference.BlazorPlayground.Utilities;

public class User
{
    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public int Age { get; set; }

    public char Gender { get; set; }

    public string? Email { get; set; }

    public string ToDisplayString() => $"{FirstName} {LastName} <{Email}>";
}