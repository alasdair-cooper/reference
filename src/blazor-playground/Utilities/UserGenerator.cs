using System.Diagnostics;
using Bogus;
using Bogus.DataSets;

namespace AlasdairCooper.Reference.BlazorPlayground.Utilities;

public static class UserGenerator
{
    public static IEnumerable<User> Users => new UserFaker().UseSeed(123).GenerateForever();
}

file sealed class UserFaker : Faker<User>
{
    public UserFaker() : base("en")
    {
        RuleFor(u => u.FirstName, (f, u) => f.Name.FirstName(u.Gender == 'F' ? Name.Gender.Female : Name.Gender.Male));
        RuleFor(u => u.LastName, f => f.Person.LastName);
        RuleFor(u => u.Age, f => f.Random.Int(18, 99));

        RuleFor(
            u => u.Gender,
            f =>
                f.Person.Gender switch
                {
                    Name.Gender.Female => 'F',
                    Name.Gender.Male => 'M',
                    _ => throw new UnreachableException()
                });

        RuleFor(u => u.Email, (f, u) => f.Internet.Email(u.FirstName, u.LastName));
    }
}