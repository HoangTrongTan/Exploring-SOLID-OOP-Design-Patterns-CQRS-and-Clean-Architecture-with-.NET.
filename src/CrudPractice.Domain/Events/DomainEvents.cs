using CrudPractice.Domain.Common;

namespace CrudPractice.Domain.Events;

/// <summary>
/// [Design Pattern - Domain Events]
/// Record → immutable (không thể thay đổi sau khi tạo).
/// Đây là event "Product đã được tạo" - dùng thì quá khứ.
///
/// [Interview term] "Domain Event" - sự kiện đã xảy ra trong domain.
/// Khác với "Command" (yêu cầu làm gì đó) vs "Event" (đã xảy ra rồi).
///
/// Implement IDomainEvent để UnitOfWork collect và dispatch qua MediatR.
/// </summary>
public record ProductCreatedEvent(
    Guid ProductId,
    string ProductName,
    decimal Price
) : IDomainEvent;

public record ProductUpdatedEvent(
    Guid ProductId,
    string ProductName
) : IDomainEvent;

public record StockChangedEvent(
    Guid ProductId,
    int NewQuantity,
    int ChangeAmount
) : IDomainEvent;

public record CategoryCreatedEvent(
    Guid CategoryId,
    string CategoryName
) : IDomainEvent;

public record CategoryUpdatedEvent(
    Guid CategoryId,
    string CategoryName
) : IDomainEvent;
