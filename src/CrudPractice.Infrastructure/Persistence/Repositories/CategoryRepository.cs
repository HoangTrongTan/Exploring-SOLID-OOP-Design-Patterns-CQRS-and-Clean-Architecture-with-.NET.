using CrudPractice.Domain.Entities;
using CrudPractice.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace CrudPractice.Infrastructure.Persistence.Repositories;

/// <summary>
/// [Design Pattern - Repository Pattern] CategoryRepository implementation.
/// </summary>
public class CategoryRepository(AppDbContext dbContext) : ICategoryRepository
{
    public async Task AddAsync(Category category, CancellationToken ct = default)
        => await dbContext.Categories.AddAsync(category, ct);

    public void Remove(Category category)
        => dbContext.Categories.Remove(category);

    public async Task<Category?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await dbContext.Categories.FindAsync([id], ct);

    public async Task<IEnumerable<Category>> GetAllAsync(CancellationToken ct = default)
        => await dbContext.Categories
            .AsNoTracking()
            .OrderBy(c => c.Name)
            .ToListAsync(ct);

    public async Task<IEnumerable<Category>> GetActiveAsync(CancellationToken ct = default)
        => await dbContext.Categories
            .Where(c => c.IsActive)
            .AsNoTracking()
            .OrderBy(c => c.Name)
            .ToListAsync(ct);

    public async Task<bool> ExistsByNameAsync(string name, CancellationToken ct = default)
        => await dbContext.Categories.AnyAsync(c => c.Name == name, ct);
}
