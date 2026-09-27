using CrudPractice.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrudPractice.Infrastructure.Persistence.Configurations;

/// <summary>
/// [EF Core Fluent API] Entity configuration cho Category.
/// Tách ra file riêng → Clean Code, Single Responsibility.
///
/// [SOLID - Open/Closed] Thêm entity mới → thêm configuration mới,
/// không cần sửa AppDbContext.
///
/// [DB Optimization] Đặt index đúng chỗ → query nhanh hơn.
/// Rule of thumb: index columns thường dùng trong WHERE, JOIN, ORDER BY.
///
/// [Interview question] "Khi nào nên đặt Index?"
/// → Khi column đó thường xuất hiện trong WHERE clause
/// → Khi query trên column đó chậm (full table scan)
/// → Nhược điểm: INSERT/UPDATE chậm hơn vì phải update index
/// </summary>
public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.ToTable("categories");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Id)
            .HasColumnName("id")
            .ValueGeneratedNever(); // Domain tự tạo Guid, DB không generate

        builder.Property(c => c.Name)
            .HasColumnName("name")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(c => c.Description)
            .HasColumnName("description")
            .HasMaxLength(500);

        builder.Property(c => c.IsActive)
            .HasColumnName("is_active")
            .HasDefaultValue(true);

        builder.Property(c => c.CreatedAt)
            .HasColumnName("created_at")
            .HasDefaultValueSql("now()"); // PostgreSQL function

        builder.Property(c => c.UpdatedAt)
            .HasColumnName("updated_at");

        // [Index - Unique] Tên category không được trùng
        // [DB Optimization] Unique index vừa đảm bảo data integrity, vừa tăng tốc lookup by name
        builder.HasIndex(c => c.Name)
            .IsUnique()
            .HasDatabaseName("ix_categories_name");

        // [Index] Filter active categories thường xuyên → index giúp nhanh hơn
        builder.HasIndex(c => c.IsActive)
            .HasDatabaseName("ix_categories_is_active");
    }
}

/// <summary>
/// [EF Core Fluent API] Entity configuration cho Product.
///
/// [DB Optimization] Composite index: khi query thường filter theo CategoryId + Price
/// → (category_id, price) composite index hiệu quả hơn 2 index đơn lẻ.
/// </summary>
public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("products");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Id)
            .HasColumnName("id")
            .ValueGeneratedNever();

        builder.Property(p => p.Name)
            .HasColumnName("name")
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(p => p.Description)
            .HasColumnName("description")
            .HasMaxLength(2000);

        // [DB Optimization] decimal(18,2) → chính xác cho tiền tệ
        // precision: tổng số chữ số, scale: số chữ số sau dấu thập phân
        builder.Property(p => p.Price)
            .HasColumnName("price")
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(p => p.StockQuantity)
            .HasColumnName("stock_quantity")
            .HasDefaultValue(0)
            .IsRequired();

        builder.Property(p => p.IsActive)
            .HasColumnName("is_active")
            .HasDefaultValue(true);

        builder.Property(p => p.CategoryId)
            .HasColumnName("category_id")
            .IsRequired();

        builder.Property(p => p.CreatedAt)
            .HasColumnName("created_at")
            .HasDefaultValueSql("now()");

        builder.Property(p => p.UpdatedAt)
            .HasColumnName("updated_at");

        // [Relationship] Product → Category: Many-to-One
        // OnDelete.Restrict → không tự động xóa product khi xóa category
        // → Phải xóa/move products trước khi xóa category
        builder.HasOne(p => p.Category)
            .WithMany()
            .HasForeignKey(p => p.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        // [Index] Unique name per product (globally)
        builder.HasIndex(p => p.Name)
            .IsUnique()
            .HasDatabaseName("ix_products_name");

        // [Composite Index] Thường query: "products trong category X, sort by price"
        // → composite index (category_id, price) rất hiệu quả
        builder.HasIndex(p => new { p.CategoryId, p.Price })
            .HasDatabaseName("ix_products_category_price");

        // [Index] Filter active products
        builder.HasIndex(p => p.IsActive)
            .HasDatabaseName("ix_products_is_active");

        // [Global Query Filter] Soft delete: mặc định không trả về inactive products
        // Nếu muốn query cả inactive → dùng .IgnoreQueryFilters()
        // [TODO - BÀI TẬP 22] Enable query filter sau khi implement soft delete
        // builder.HasQueryFilter(p => p.IsActive);
    }
}
