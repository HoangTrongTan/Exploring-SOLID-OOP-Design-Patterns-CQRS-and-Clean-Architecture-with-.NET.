using CrudPractice.Domain.Entities;
using CrudPractice.Domain.Interfaces.Repositories;
using Dapper;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace CrudPractice.Infrastructure.Persistence.Repositories;

/// <summary>
/// [Design Pattern - Repository Pattern] Implementation cụ thể.
/// Domain layer chỉ biết interface IProductRepository,
/// Infrastructure layer biết EF Core và Dapper.
///
/// [Strategy Pattern for DB access]
/// ┌──────────────────┬──────────────────────────────────┐
/// │ EF Core          │ WRITE: Insert, Update, Delete    │
/// │ Dapper Raw SQL   │ READ: Complex queries, pagination│
/// └──────────────────┴──────────────────────────────────┘
///
/// Lý do dùng cả 2:
///   - EF Core: Change tracking, relationships, migrations
///   - Dapper: Performance cho queries phức tạp, multi-join, aggregation
///
/// [Interview question] "EF Core vs Dapper khác nhau như thế nào?"
/// → EF Core: ORM đầy đủ, change tracking, migrations, LINQ to SQL
///   Dapper: Micro-ORM, raw SQL, fast, no change tracking
///   Khi nào dùng Dapper: reporting queries, complex JOINs, performance-critical reads
/// </summary>
public class ProductRepository(
    AppDbContext dbContext,
    NpgsqlDataSource dataSource // Dapper cần raw connection pool
) : IProductRepository
{
    // ── EF Core: Write operations ─────────────────────────────────────

    public async Task AddAsync(Product product, CancellationToken ct = default)
        => await dbContext.Products.AddAsync(product, ct);

    public void Remove(Product product)
        => dbContext.Products.Remove(product);

    // ── Dapper: Optimized Read operations ────────────────────────────

    /// <summary>
    /// [TODO - BÀI TẬP 8] Implement GetByIdAsync.
    ///
    /// Hint SQL:
    ///   SELECT p.*, c.name AS category_name
    ///   FROM products p
    ///   LEFT JOIN categories c ON p.category_id = c.id
    ///   WHERE p.id = @Id
    ///
    /// Hint: Dùng Dapper QuerySingleOrDefaultAsync hoặc EF Core với Include.
    ///
    /// [Interview question] "Eager Loading vs Lazy Loading vs Explicit Loading trong EF Core?"
    /// → Eager: .Include() → load related data ngay trong 1 query
    ///   Lazy: load khi access navigation property (cần virtual + UseLazyLoadingProxies)
    ///   Explicit: DbContext.Entry(entity).Reference().LoadAsync()
    ///   N+1 problem: Lazy loading có thể gây N+1 queries → prefer Eager loading
    /// </summary>
    public async Task<Product?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        // Option A: EF Core với Eager Loading (dễ implement, đủ dùng)
        return await dbContext.Products
            .Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.Id == id, ct);

        // [TODO] Option B: Dapper raw SQL (uncomment khi học Dapper)
        // const string sql = """
        //     SELECT p.id AS Id, p.name AS Name, ...
        //     FROM products p
        //     LEFT JOIN categories c ON p.category_id = c.id
        //     WHERE p.id = @Id
        //     """;
        // await using var conn = await dataSource.OpenConnectionAsync(ct);
        // ...
    }

    /// <summary>
    /// [TODO - BÀI TẬP 8] Implement GetAllAsync với pagination.
    ///
    /// [DB Optimization] OFFSET-based pagination:
    ///   LIMIT @PageSize OFFSET (@PageNumber - 1) * @PageSize
    ///
    /// [Interview question] "OFFSET pagination có vấn đề gì với large dataset?"
    /// → Vấn đề: OFFSET 100000 → DB vẫn phải đọc 100000 rows rồi skip
    ///   Solution: Cursor-based pagination (keyset pagination)
    ///   WHERE id > @LastId ORDER BY id LIMIT @PageSize
    ///   → Hiệu quả hơn vì dùng index, không scan toàn bộ rows trước đó
    /// </summary>
    public async Task<IEnumerable<Product>> GetAllAsync(
        int pageNumber, int pageSize, CancellationToken ct = default)
    {
        // [TODO] Implement với Dapper hoặc EF Core AsNoTracking
        // [DB Optimization] AsNoTracking() → không track entities → nhanh hơn cho read-only
        return await dbContext.Products
            .Include(p => p.Category)
            .AsNoTracking()
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);
    }

    public async Task<int> CountAsync(CancellationToken ct = default)
        => await dbContext.Products.CountAsync(ct);

    public async Task<IEnumerable<Product>> GetByCategoryIdAsync(
        Guid categoryId, CancellationToken ct = default)
    {
        // [TODO - BÀI TẬP 23] Implement với raw Dapper SQL
        return await dbContext.Products
            .Include(p => p.Category)
            .Where(p => p.CategoryId == categoryId)
            .AsNoTracking()
            .ToListAsync(ct);
    }

    public async Task<bool> ExistsByNameAsync(string name, CancellationToken ct = default)
        => await dbContext.Products.AnyAsync(p => p.Name == name, ct);
}
