namespace CrudPractice.Domain.Interfaces;

/// <summary>
/// [Design Pattern - Unit of Work]
/// Quản lý transaction: nhiều repository operations trong 1 transaction duy nhất.
/// Đảm bảo atomicity: hoặc tất cả thành công, hoặc rollback hết.
///
/// Ví dụ CRUD flow:
///   productRepo.AddAsync(product)       → track entity
///   categoryRepo.Update(category)       → track change
///   await unitOfWork.SaveChangesAsync() → 1 DB transaction
///
/// [SOLID - Interface Segregation]
/// IUnitOfWork không expose DbContext → Application layer không biết EF Core tồn tại.
///
/// [Interview question] "Unit of Work Pattern là gì?"
/// → Trả lời: duy trì list các operations cần được commit,
///   khi SaveChanges được gọi thì tất cả operations được thực thi trong 1 transaction.
/// </summary>
public interface IUnitOfWork : IAsyncDisposable
{
    /// <summary>
    /// Persist tất cả tracked changes vào DB trong 1 transaction.
    /// Trả về số rows bị affected.
    /// </summary>
    Task<int> SaveChangesAsync(CancellationToken ct = default);

    /// <summary>
    /// Persist changes + dispatch domain events qua MediatR.
    /// Dùng khi operation cần notify các handlers khác (email, cache, log).
    ///
    /// [Pattern - Transactional Outbox simplified]
    /// Production: dùng Outbox pattern đầy đủ để đảm bảo at-least-once delivery.
    /// </summary>
    Task SaveChangesAndDispatchEventsAsync(CancellationToken ct = default);
}
