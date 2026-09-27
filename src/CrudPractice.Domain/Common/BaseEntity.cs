namespace CrudPractice.Domain.Common;

/// <summary>
/// [SOLID - Single Responsibility Principle]
/// BaseEntity chỉ chịu trách nhiệm:
///   1. Quản lý định danh (Id)
///   2. Tracking timestamps (CreatedAt, UpdatedAt)
///   3. Thu thập Domain Events
///
/// [OOP - Abstraction] abstract class → không thể new BaseEntity() trực tiếp.
/// Chỉ các class con (Product, Category...) mới được instantiate.
///
/// [OOP - Encapsulation] Setter là protected/private → bên ngoài không thể
/// set trực tiếp mà phải qua method của entity.
///
/// [Design Pattern - Template Method] SetUpdatedAt() là protected → chỉ entity
/// con mới gọi được, ép buộc đúng flow update.
///
/// [DDD - Aggregate Root] Mọi entity trong DDD đều có Id riêng.
/// Không dùng DB auto-increment → Domain tự tạo Id (Guid.NewGuid()).
/// Lý do: tránh phụ thuộc DB, dễ test, dễ distributed system.
/// </summary>
public abstract class BaseEntity
{
    // [OOP - Encapsulation] protected set → chỉ class này và class con mới set được
    public Guid Id { get; protected set; } = Guid.NewGuid();
    public DateTime CreatedAt { get; protected set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; protected set; }

    // [Design Pattern - Observer qua Domain Events]
    // private → bên ngoài không thể add event vào trực tiếp
    // IReadOnlyCollection → expose ra ngoài read-only (Liskov Substitution)
    private readonly List<IDomainEvent> _domainEvents = new();

    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    /// <summary>
    /// [SOLID - Open/Closed] Entity con có thể raise thêm event khác
    /// mà không cần sửa BaseEntity.
    /// </summary>
    protected void AddDomainEvent(IDomainEvent domainEvent)
        => _domainEvents.Add(domainEvent);

    /// <summary>
    /// Được gọi bởi UnitOfWork sau khi SaveChanges + publish events.
    /// Tránh publish 2 lần nếu SaveChanges được gọi nhiều lần.
    /// </summary>
    public void ClearDomainEvents()
        => _domainEvents.Clear();

    /// <summary>
    /// [Template Method] Chỉ entity con mới được gọi khi thực hiện update.
    /// Đảm bảo UpdatedAt luôn được set đúng theo UTC.
    /// </summary>
    protected void SetUpdatedAt()
        => UpdatedAt = DateTime.UtcNow;
}
