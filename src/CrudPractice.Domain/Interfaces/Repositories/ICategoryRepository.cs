using CrudPractice.Domain.Entities;

namespace CrudPractice.Domain.Interfaces.Repositories;

/// <summary>
/// [Design Pattern - Repository Pattern]
/// ICategoryRepository cho Category aggregate root.
///
/// [SOLID - Interface Segregation]
/// Tách riêng khỏi IProductRepository → mỗi interface nhỏ, rõ ràng.
/// </summary>
public interface ICategoryRepository
{
    Task<Category?> GetByIdAsync(Guid id, CancellationToken ct = default);

    Task<IEnumerable<Category>> GetAllAsync(CancellationToken ct = default);

    Task<IEnumerable<Category>> GetActiveAsync(CancellationToken ct = default);

    Task<bool> ExistsByNameAsync(string name, CancellationToken ct = default);

    Task AddAsync(Category category, CancellationToken ct = default);

    void Remove(Category category);
}
