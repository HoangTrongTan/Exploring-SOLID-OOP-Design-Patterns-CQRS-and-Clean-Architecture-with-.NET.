using CrudPractice.Application.DTOs;
using CrudPractice.Domain.Interfaces.Repositories;
using MediatR;

namespace CrudPractice.Application.Features.Categories.Queries;

public record GetCategoriesQuery : IRequest<IEnumerable<CategoryDto>>;

public record GetCategoryByIdQuery(Guid Id) : IRequest<CategoryDto>;

/// <summary>
/// [TODO - BÀI TẬP 20] Implement GetCategoriesQueryHandler.
/// </summary>
public class GetCategoriesQueryHandler(
    ICategoryRepository categoryRepository
) : IRequestHandler<GetCategoriesQuery, IEnumerable<CategoryDto>>
{
    public Task<IEnumerable<CategoryDto>> Handle(
        GetCategoriesQuery request,
        CancellationToken ct)
    {
        throw new NotImplementedException("BÀI TẬP 20: Implement GetCategoriesQueryHandler");
    }
}

/// <summary>
/// [TODO - BÀI TẬP 21] Implement GetCategoryByIdQueryHandler.
/// </summary>
public class GetCategoryByIdQueryHandler(
    ICategoryRepository categoryRepository
) : IRequestHandler<GetCategoryByIdQuery, CategoryDto>
{
    public Task<CategoryDto> Handle(
        GetCategoryByIdQuery request,
        CancellationToken ct)
    {
        throw new NotImplementedException("BÀI TẬP 21: Implement GetCategoryByIdQueryHandler");
    }
}
