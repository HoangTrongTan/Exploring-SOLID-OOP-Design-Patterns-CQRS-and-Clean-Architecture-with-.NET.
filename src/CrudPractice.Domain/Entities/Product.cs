using CrudPractice.Domain.Common;
using CrudPractice.Domain.Exceptions;

namespace CrudPractice.Domain.Entities;

/// <summary>
/// [DDD - Aggregate Root] Product là Aggregate Root.
///
/// [OOP - Encapsulation]
/// Tất cả properties đều private setter.
/// Business rules được enforce qua domain methods.
///
/// [SOLID - Single Responsibility]
/// Product chỉ biết về business logic của chính nó.
/// Không biết DB, không biết HTTP, không biết cache.
///
/// [Design Pattern - Rich Domain Model]
/// Thay vì Anemic Domain Model (chỉ có data, không có behavior),
/// Product có đủ behavior methods encapsulate business logic.
/// </summary>
public class Product : BaseEntity
{
    public string Name { get; private set; } = default!;
    public string? Description { get; private set; }

    /// <summary>
    /// [DB Optimization] Dùng decimal cho tiền tệ - KHÔNG dùng float/double
    /// Vì float/double có precision issue: 0.1 + 0.2 ≠ 0.3 trong float
    /// decimal(18,2) → lưu chính xác đến 2 chữ số thập phân
    /// </summary>
    public decimal Price { get; private set; }

    public int StockQuantity { get; private set; }
    public bool IsActive { get; private set; }

    // [FK] Reference đến Category qua Id (không navigate trực tiếp sang aggregate khác)
    public Guid CategoryId { get; private set; }

    // [EF Core Navigation Property] Nullable vì lazy load
    // [DDD] Product là Aggregate Root, Category là separate aggregate
    // → Reference qua Id, không phải object reference trực tiếp
    public Category? Category { get; private set; }

    // [EF Core requirement]
    protected Product() { }

    // ╔══════════════════════════════════════════════════╗
    // ║           FACTORY METHOD PATTERN                ║
    // ╚══════════════════════════════════════════════════╝

    /// <summary>
    /// [TODO - BÀI TẬP 4] Implement Product.Create() factory method.
    /// Business rules:
    ///   - name: không rỗng, 2-200 ký tự
    ///   - price: phải >= 0
    ///   - stockQuantity: phải >= 0
    ///   - categoryId: không được là Guid.Empty
    ///   - IsActive mặc định = true
    ///   - Raise: ProductCreatedEvent
    /// </summary>
    public static Product Create(
        string name,
        string? description,
        decimal price,
        int stockQuantity,
        Guid categoryId)
    {
        // TODO: Viết code tại đây
        throw new NotImplementedException("BÀI TẬP 4: Implement Product.Create()");
    }

    // ╔══════════════════════════════════════════════════╗
    // ║              BEHAVIOR METHODS                   ║
    // ╚══════════════════════════════════════════════════╝

    /// <summary>
    /// [TODO - BÀI TẬP 5] Implement UpdateInfo method.
    /// Business rules: giống Create, gọi SetUpdatedAt(), raise ProductUpdatedEvent.
    /// </summary>
    public void UpdateInfo(string name, string? description, decimal price, Guid categoryId)
    {
        // TODO: Viết code tại đây
        throw new NotImplementedException("BÀI TẬP 5: Implement Product.UpdateInfo()");
    }

    /// <summary>
    /// [TODO - BÀI TẬP 6] Implement AddStock method.
    /// Business rules:
    ///   - quantity phải > 0
    ///   - StockQuantity += quantity
    ///   - Gọi SetUpdatedAt()
    ///   - Raise: StockChangedEvent
    /// </summary>
    public void AddStock(int quantity)
    {
        // TODO: Viết code tại đây
        throw new NotImplementedException("BÀI TẬP 6: Implement Product.AddStock()");
    }

    /// <summary>
    /// [TODO - BÀI TẬP 7] Implement ReduceStock method.
    /// Business rules:
    ///   - quantity phải > 0
    ///   - Nếu StockQuantity - quantity < 0 → throw DomainException (insufficient stock)
    ///   - StockQuantity -= quantity
    ///   - Gọi SetUpdatedAt()
    /// </summary>
    public void ReduceStock(int quantity)
    {
        // TODO: Viết code tại đây
        throw new NotImplementedException("BÀI TẬP 7: Implement Product.ReduceStock()");
    }

    public void Deactivate()
    {
        if (!IsActive) return;
        IsActive = false;
        SetUpdatedAt();
    }
}
