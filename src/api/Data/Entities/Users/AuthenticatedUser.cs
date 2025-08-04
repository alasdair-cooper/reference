using System.ComponentModel.DataAnnotations;
using AlasdairCooper.Reference.Api.Data.Entities.Addresses;

namespace AlasdairCooper.Reference.Api.Data.Entities.Users;

public sealed class AuthenticatedUser(int id, string firstName, string[] middleNames, string? lastName) : User(id)
{
    [StringLength(100)]
    public string FirstName { get; init; } = firstName;

    public string[] MiddleNames { get; init; } = middleNames;

    [StringLength(200)]
    public string? LastName { get; init; } = lastName;

    public Address? DeliveryAddress { get; private set; }

    public List<Address> Addresses { get; private set; } = null!;

    public void AddAddress(Address address)
    {
        // ReSharper disable once NullCoalescingConditionIsAlwaysNotNullAccordingToAPIContract
        Addresses ??= [];
        Addresses.Add(address);
        DeliveryAddress ??= address;
    }

    public string FullName =>
        (FirstName, MiddleNames, LastName) switch
        {
            (var first, [], null) => first,
            (var first, [], var last) => $"{first} {last}",
            (var first, var middle, null) => $"{first} {string.Join(' ', middle)}",
            var (first, middle, last) => $"{first} {string.Join(' ', middle)} {last}"
        };

    public string? Initials =>
        (FirstName, LastName) switch
        {
            ([var first, ..], null) => char.ToUpper(first).ToString(),
            ([var first, ..], [var last, ..]) => $"{char.ToUpper(first)}{char.ToUpper(last)}",
            (_, _) => null
        };
}