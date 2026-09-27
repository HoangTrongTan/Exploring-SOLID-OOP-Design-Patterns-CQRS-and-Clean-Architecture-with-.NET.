using CrudPractice.Domain.Entities;

namespace CrudPractice.Domain.Interfaces.Repositories;

/// <summary>
/// [Design Pattern - Repository Pattern]
/// Interface định nghĩa "hợp đồng" với DB layer.
/// Domain layer chỉ biết interface → không biết EF Core hay Dapper.
///
/// [SOLID - Dependency Inversion Principle]
/// High-level module (Application) phụ thuộc vào abstraction (IProductRepository),
/// KHÔNG phụ thuộc vào low-level module (ProductRepository).
///
/// [SOLID - Interface Segregation Principle]
/// Tách IProductRepository riêng thay vì 1 IRepository to.
/// Mỗi repository chỉ có những method cần thiết cho entity đó.
///
/// [Interview question] "Repository Pattern là gì? Tại sao lại dùng?"
/// → Trả lời: abstraction layer between business logic and data access.
///   Dễ test (mock repository), dễ thay DB (swap implementation).
/// </summary>
public interface IProductRepository
{
    // ── Read operations (sẽ dùng Dapper cho performance) ──────────────

    /// <summary>
    /// [TODO - BÀI TẬP 8] Implement trong ProductRepository.
    /// Hint: Dùng Dapper với raw SQL, JOIN với categories table.
    /// </summary>
    Task<Product?> GetByIdAsync(Guid id, CancellationToken ct = default);

    /// <summary>
    /// [Pagination] Phân trang sản phẩm.
    /// pageNumber bắt đầu từ 1.
    /// </summary>
    Task<IEnumerable<Product>> GetAllAsync(int pageNumber, int pageSize, CancellationToken ct = default);

    Task<int> CountAsync(CancellationToken ct = default);

    Task<IEnumerable<Product>> GetByCategoryIdAsync(Guid categoryId, CancellationToken ct = default);

    Task<bool> ExistsByNameAsync(string name, CancellationToken ct = default);

    // ── Write operations (sẽ dùng EF Core) ───────────────────────────

    Task AddAsync(Product product, CancellationToken ct = default);

    // [Note] Update và Delete không cần method riêng trong Repository
    // vì EF Core Change Tracker tự detect thay đổi khi gọi SaveChanges.
    // Product đã được track bởi DbContext → chỉ cần modify entity + SaveChanges.
    //
    // Hard delete nếu cần:
    void Remove(Product product);
}
