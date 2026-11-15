namespace BandUp.Domain.Common;

/// <summary>
/// Base type for all entities. IDs are UUIDv7: globally unique, safe to expose,
/// and time-ordered so B-tree indexes stay efficient.
/// </summary>
public abstract class Entity
{
    public Guid Id { get; protected init; } = Guid.CreateVersion7();

    public DateTimeOffset CreatedAt { get; protected init; } = DateTimeOffset.UtcNow;
}
