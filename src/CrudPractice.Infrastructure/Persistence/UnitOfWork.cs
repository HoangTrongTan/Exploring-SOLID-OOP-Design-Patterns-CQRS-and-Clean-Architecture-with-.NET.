using CrudPractice.Domain.Common;
using CrudPractice.Domain.Interfaces;
using MediatR;

namespace CrudPractice.Infrastructure.Persistence;

/// <summary>
/// [Design Pattern - Unit of Work]
/// Implementation của IUnitOfWork.
/// Bao bọc DbContext, quản lý transaction và dispatch domain events.
/// </summary>
public sealed class UnitOfWork(
    AppDbContext dbContext,
    IPublisher publisher // MediatR IPublisher để dispatch domain events
) : IUnitOfWork
{
    public async Task<int> SaveChangesAsync(CancellationToken ct = default)
        => await dbContext.SaveChangesAsync(ct);

    public async Task SaveChangesAndDispatchEventsAsync(CancellationToken ct = default)
    {
        // Thu thập domain events TRƯỚC khi save
        var domainEvents = dbContext.ChangeTracker
            .Entries<BaseEntity>()
            .SelectMany(e => e.Entity.DomainEvents)
            .ToList();

        await dbContext.SaveChangesAsync(ct);

        // Clear events sau khi save thành công
        foreach (var entry in dbContext.ChangeTracker.Entries<BaseEntity>())
            entry.Entity.ClearDomainEvents();

        // Publish từng event qua MediatR
        foreach (var domainEvent in domainEvents)
        {
            if (domainEvent is INotification notification)
                await publisher.Publish(notification, ct);
        }
    }

    public async ValueTask DisposeAsync()
        => await dbContext.DisposeAsync();
}
