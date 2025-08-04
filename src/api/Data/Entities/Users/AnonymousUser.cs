namespace AlasdairCooper.Reference.Api.Data.Entities.Users;

public sealed class AnonymousUser(int id, string correlationId) : User(id)
{
    public string CorrelationId { get; init; } = correlationId;
}