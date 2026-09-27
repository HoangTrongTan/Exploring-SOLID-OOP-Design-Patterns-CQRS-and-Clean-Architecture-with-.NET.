using CrudPractice.Application.DTOs;
using CrudPractice.Domain.Interfaces.Repositories;
using FluentValidation;
using MediatR;

namespace CrudPractice.Application.Features.Products.Queries;

// ╔══════════════════════════════════════════════════╗
// ║          GET ALL PRODUCTS (PAGINATED)           ║
// ╚══════════════════════════════════════════════════╝

/// <summary>
/// [CQRS - Query] Lấy danh sách sản phẩm có phân trang.
///
/// [Design Pattern - CQRS]
/// Command: thay đổi state (Create, Update, Delete)
/// Query: chỉ đọc, không thay đổi state
///
/// Record → immutable → thread-safe → ideal cho Command/Query objects
///
/// [Interview term] "CQRS - Command Query Responsibility Segregation"
/// Tách read model và write model → optimize độc lập.
/// Query side có thể dùng Dapper (fast read), Command side dùng EF Core.
/// </summary>
public record GetProductsQuery(
    int PageNumber = 1,
    int PageSize = 10,
    Guid? CategoryId = null
) : IRequest<PagedResult<ProductSummaryDto>>;

public class GetProductsValidator : AbstractValidator<GetProductsQuery>
{
    public GetProductsValidator()
    {
        RuleFor(x => x.PageNumber).GreaterThanOrEqualTo(1);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
    }
}

/// <summary>
/// [TODO - BÀI TẬP 10] Implement GetProductsQueryHandler.
/// Yêu cầu:
///   - Dùng IProductRepository.GetAllAsync() với pagination
///   - Map entities → ProductSummaryDto
///   - Trả về PagedResult với TotalCount
///   - Nếu CategoryId != null → filter theo category
/// </summary>
public class GetProductsQueryHandler(
    IProductRepository productRepository
// TODO: inject IMapper
) : IRequestHandler<GetProductsQuery, PagedResult<ProductSummaryDto>>
{
    public Task<PagedResult<ProductSummaryDto>> Handle(
        GetProductsQuery request,
        CancellationToken ct)
    {
        // TODO: Implement
        throw new NotImplementedException("BÀI TẬP 10: Implement GetProductsQueryHandler");
    }
}

// ╔══════════════════════════════════════════════════╗
// ║            GET PRODUCT BY ID                   ║
// ╚══════════════════════════════════════════════════╝

public record GetProductByIdQuery(Guid Id) : IRequest<ProductDto>;

/// <summary>
/// [TODO - BÀI TẬP 11] Implement GetProductByIdQueryHandler.
/// Yêu cầu:
///   - Nếu không tìm thấy → throw NotFoundException("Product", request.Id)
///   - Map entity → ProductDto
/// </summary>
public class GetProductByIdQueryHandler(
    IProductRepository productRepository
// TODO: inject IMapper
) : IRequestHandler<GetProductByIdQuery, ProductDto>
{
    public Task<ProductDto> Handle(
        GetProductByIdQuery request,
        CancellationToken ct)
    {
        // TODO: Implement
        throw new NotImplementedException("BÀI TẬP 11: Implement GetProductByIdQueryHandler");
    }
}
