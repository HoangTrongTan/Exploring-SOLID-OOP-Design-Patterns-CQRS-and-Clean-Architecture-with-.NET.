using CrudPractice.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CrudPractice.Infrastructure.Persistence;

/// <summary>
/// [EF Core - DbContext] Entry point cho tất cả DB operations.
///
/// [Design Pattern - Repository of Repositories]
/// DbContext là implementation detail của Repository Pattern.
/// Application layer không biết DbContext tồn tại.
///
/// [Code First] Schema được define từ code (Fluent API configuration).
/// Không dùng DataAnnotations trong Entity → Domain không coupling với EF Core.
///
/// [Primary Constructor] C# 12 syntax → gọn hơn so với traditional constructor.
/// </summary>
public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    // [EF Core - DbSet] Represents tables in DB
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Category> Categories => Set<Category>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // [EF Core] Tự scan và apply tất cả IEntityTypeConfiguration trong assembly
        // Không cần gọi từng configuration một → maintainable, scalable
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        // [Convention] Tất cả string mặc định varchar(255) thay vì nvarchar(max)
        // Tiết kiệm storage, tốt hơn cho indexing
        configurationBuilder.Properties<string>()
            .HaveMaxLength(255);

        // [Convention] DateTime luôn store UTC
        // Tránh bug timezone khi deploy lên server khác múi giờ
        configurationBuilder.Properties<DateTime>()
            .HaveConversion<DateTimeUtcConverter>();
    }
}

/// <summary>
/// [EF Core Value Converter] Đảm bảo DateTime luôn được lưu dưới dạng UTC.
///
/// [Interview question] "Tại sao phải store DateTime dưới dạng UTC?"
/// → Trả lời: Server có thể ở timezone khác client. UTC là chuẩn toàn cầu.
///   Client nhận UTC → tự convert sang local timezone để display.
/// </summary>
public class DateTimeUtcConverter()
    : Microsoft.EntityFrameworkCore.Storage.ValueConversion.ValueConverter<DateTime, DateTime>(
        v => v.Kind == DateTimeKind.Utc ? v : v.ToUniversalTime(),
        v => DateTime.SpecifyKind(v, DateTimeKind.Utc));
