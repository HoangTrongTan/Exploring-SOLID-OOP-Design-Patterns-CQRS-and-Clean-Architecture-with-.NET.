using CrudPractice.Domain.Common;
using CrudPractice.Domain.Exceptions;

namespace CrudPractice.Domain.Entities;

/// <summary>
/// [DDD - Aggregate Root] Category là Aggregate Root.
/// Product sẽ tham chiếu đến Category qua CategoryId (FK).
///
/// [OOP - Encapsulation] Tất cả setters là private.
/// Chỉ có thể thay đổi state qua method của chính entity.
///
/// [Design Pattern - Factory Method] Static Create() thay vì constructor.
/// Lý do:
///   1. Constructor không thể throw exception cleanly.
///   2. Factory method có tên gọi rõ ràng.
///   3. Có thể validate input trước khi tạo object.
/// </summary>
public class Category : BaseEntity
{
    // [OOP - Encapsulation] private set → chỉ Category mới set được
    public string Name { get; private set; } = default!;
    public string? Description { get; private set; }
    public bool IsActive { get; private set; }

    // [EF Core requirement] Protected parameterless constructor
    // EF Core cần constructor không tham số để materialize entities từ DB
    // protected → tránh code bên ngoài vô tình new Category() mà không qua Factory
    protected Category() { }

    // ╔══════════════════════════════════════════════════╗
    // ║           FACTORY METHOD PATTERN                ║
    // ╚══════════════════════════════════════════════════╝

    /// <summary>
    /// [TODO - BÀI TẬP 1] Implement factory method này.
    /// Yêu cầu:
    ///   - Validate: name không được rỗng, độ dài 2-100 ký tự
    ///   - Raise domain event: CategoryCreatedEvent
    ///   - IsActive mặc định = true
    /// </summary>
    public static Category Create(string name, string? description = null)
    {
        // TODO: Viết code tại đây
        throw new NotImplementedException("BÀI TẬP 1: Implement Category.Create()");
    }

    // ╔══════════════════════════════════════════════════╗
    // ║              BEHAVIOR METHODS                   ║
    // ╚══════════════════════════════════════════════════╝

    /// <summary>
    /// [TODO - BÀI TẬP 2] Implement Update method.
    /// Yêu cầu:
    ///   - Validate name giống Create
    ///   - Gọi SetUpdatedAt()
    ///   - Raise domain event: CategoryUpdatedEvent
    /// </summary>
    public void Update(string name, string? description)
    {
        // TODO: Viết code tại đây
        throw new NotImplementedException("BÀI TẬP 2: Implement Category.Update()");
    }

    /// <summary>
    /// [TODO - BÀI TẬP 3] Implement Deactivate method.
    /// Yêu cầu:
    ///   - Idempotent: nếu đã deactivated rồi thì không làm gì
    ///   - Gọi SetUpdatedAt()
    /// </summary>
    public void Deactivate()
    {
        // TODO: Viết code tại đây
        throw new NotImplementedException("BÀI TẬP 3: Implement Category.Deactivate()");
    }

    public void Activate()
    {
        if (IsActive) return; // Idempotent
        IsActive = true;
        SetUpdatedAt();
    }
}
