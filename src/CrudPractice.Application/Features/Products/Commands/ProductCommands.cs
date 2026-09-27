using CrudPractice.Application.DTOs;
using CrudPractice.Domain.Exceptions;
using CrudPractice.Domain.Interfaces;
using CrudPractice.Domain.Interfaces.Repositories;
using FluentValidation;
using MediatR;

namespace CrudPractice.Application.Features.Products.Commands;

// ╔══════════════════════════════════════════════════╗
// ║              CREATE PRODUCT                     ║
// ╚══════════════════════════════════════════════════╝

/// <summary>
/// [CQRS - Command] Command tạo Product mới.
///
/// [Design Pattern - Command Pattern]
/// Encapsulate request như 1 object → có thể queue, log, undo.
///
/// Record → immutable → Command không thể bị thay đổi sau khi dispatch.
/// </summary>
public record CreateProductCommand(
    string Name,
    string? Description,
    decimal Price,
    int StockQuantity,
    Guid CategoryId
) : IRequest<ProductDto>;

/// <summary>
/// [FluentValidation] Validator cho CreateProductCommand.
/// Đặt validation rules tại Application layer, không phải Controller.
///
/// [SOLID - Single Responsibility]
/// Validator chỉ lo validate input.
/// Handler chỉ lo business logic.
/// </summary>
public class CreateProductValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Product name is required.")
            .MinimumLength(2).WithMessage("Name must be at least 2 characters.")
            .MaximumLength(200);

        // [TODO - BÀI TẬP 12] Thêm validation rules cho:
        // - Price: >= 0
        // - StockQuantity: >= 0
        // - CategoryId: không được là Guid.Empty
    }
}

/// <summary>
/// [TODO - BÀI TẬP 13] Implement CreateProductCommandHandler.
/// Yêu cầu:
///   1. Kiểm tra category có tồn tại không → NotFoundException nếu không
///   2. Kiểm tra tên product đã tồn tại chưa → DomainException nếu trùng
///   3. Gọi Product.Create() factory method
///   4. productRepository.AddAsync()
///   5. unitOfWork.SaveChangesAndDispatchEventsAsync()
///   6. Map entity → ProductDto và return
/// </summary>
public class CreateProductCommandHandler(
    IProductRepository productRepository,
    ICategoryRepository categoryRepository,
    IUnitOfWork unitOfWork
// TODO: inject IMapper
) : IRequestHandler<CreateProductCommand, ProductDto>
{
    public Task<ProductDto> Handle(
        CreateProductCommand request,
        CancellationToken ct)
    {
        // TODO: Implement
        throw new NotImplementedException("BÀI TẬP 13: Implement CreateProductCommandHandler");
    }
}

// ╔══════════════════════════════════════════════════╗
// ║              UPDATE PRODUCT                     ║
// ╚══════════════════════════════════════════════════╝

public record UpdateProductCommand(
    Guid Id,
    string Name,
    string? Description,
    decimal Price,
    Guid CategoryId
) : IRequest<ProductDto>;

public class UpdateProductValidator : AbstractValidator<UpdateProductCommand>
{
    public UpdateProductValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MinimumLength(2).MaximumLength(200);
        RuleFor(x => x.Price).GreaterThanOrEqualTo(0);
        RuleFor(x => x.CategoryId).NotEmpty();
    }
}

/// <summary>
/// [TODO - BÀI TẬP 14] Implement UpdateProductCommandHandler.
/// Yêu cầu:
///   1. GetByIdAsync → NotFoundException nếu không tìm thấy
///   2. Kiểm tra category mới có tồn tại không
///   3. product.UpdateInfo() → domain method
///   4. unitOfWork.SaveChangesAndDispatchEventsAsync()
///   5. Return updated ProductDto
/// </summary>
public class UpdateProductCommandHandler(
    IProductRepository productRepository,
    ICategoryRepository categoryRepository,
    IUnitOfWork unitOfWork
// TODO: inject IMapper
) : IRequestHandler<UpdateProductCommand, ProductDto>
{
    public Task<ProductDto> Handle(
        UpdateProductCommand request,
        CancellationToken ct)
    {
        // TODO: Implement
        throw new NotImplementedException("BÀI TẬP 14: Implement UpdateProductCommandHandler");
    }
}

// ╔══════════════════════════════════════════════════╗
// ║              DELETE PRODUCT                     ║
// ╚══════════════════════════════════════════════════╝

/// <summary>
/// [CQRS] Delete command không trả về data → IRequest (không có generic type).
/// Trả về bool để biết có xóa thành công không.
/// </summary>
public record DeleteProductCommand(Guid Id) : IRequest<bool>;

/// <summary>
/// [TODO - BÀI TẬP 15] Implement DeleteProductCommandHandler.
/// Yêu cầu:
///   1. GetByIdAsync → NotFoundException nếu không tìm thấy
///   2. Soft delete: product.Deactivate() → IsActive = false
///      HOẶC hard delete: productRepository.Remove(product)
///   3. unitOfWork.SaveChangesAsync()
///   4. Return true
///
/// [Interview question] "Soft delete vs Hard delete khác nhau như thế nào?"
/// → Soft: set IsDeleted = true, data vẫn còn trong DB (có thể restore, audit)
///   Hard: DELETE FROM table WHERE id = ?, data mất hoàn toàn
/// </summary>
public class DeleteProductCommandHandler(
    IProductRepository productRepository,
    IUnitOfWork unitOfWork
) : IRequestHandler<DeleteProductCommand, bool>
{
    public Task<bool> Handle(
        DeleteProductCommand request,
        CancellationToken ct)
    {
        // TODO: Implement
        throw new NotImplementedException("BÀI TẬP 15: Implement DeleteProductCommandHandler");
    }
}

// ╔══════════════════════════════════════════════════╗
// ║              ADJUST STOCK                       ║
// ╚══════════════════════════════════════════════════╝

public record AdjustStockCommand(
    Guid ProductId,
    int Quantity,
    bool IsAddition // true = add stock, false = reduce stock
) : IRequest<ProductDto>;

/// <summary>
/// [TODO - BÀI TẬP 16] Implement AdjustStockCommandHandler.
/// Yêu cầu:
///   - Nếu IsAddition: product.AddStock(request.Quantity)
///   - Nếu !IsAddition: product.ReduceStock(request.Quantity)
///   - SaveChanges
/// </summary>
public class AdjustStockCommandHandler(
    IProductRepository productRepository,
    IUnitOfWork unitOfWork
// TODO: inject IMapper
) : IRequestHandler<AdjustStockCommand, ProductDto>
{
    public Task<ProductDto> Handle(
        AdjustStockCommand request,
        CancellationToken ct)
    {
        // TODO: Implement
        throw new NotImplementedException("BÀI TẬP 16: Implement AdjustStockCommandHandler");
    }
}
