using CrudPractice.Application.DTOs;
using CrudPractice.Domain.Interfaces;
using CrudPractice.Domain.Interfaces.Repositories;
using FluentValidation;
using MediatR;

namespace CrudPractice.Application.Features.Categories.Commands;

// ╔══════════════════════════════════════════════════╗
// ║            CREATE CATEGORY                      ║
// ╚══════════════════════════════════════════════════╝

public record CreateCategoryCommand(
    string Name,
    string? Description
) : IRequest<CategoryDto>;

public class CreateCategoryValidator : AbstractValidator<CreateCategoryCommand>
{
    public CreateCategoryValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MinimumLength(2)
            .MaximumLength(100);
    }
}

/// <summary>
/// [TODO - BÀI TẬP 17] Implement CreateCategoryCommandHandler.
/// Yêu cầu:
///   1. Kiểm tra tên đã tồn tại chưa → DomainException
///   2. Category.Create()
///   3. categoryRepository.AddAsync()
///   4. unitOfWork.SaveChangesAndDispatchEventsAsync()
///   5. Map → CategoryDto và return
/// </summary>
public class CreateCategoryCommandHandler(
    ICategoryRepository categoryRepository,
    IUnitOfWork unitOfWork
// TODO: inject IMapper
) : IRequestHandler<CreateCategoryCommand, CategoryDto>
{
    public Task<CategoryDto> Handle(
        CreateCategoryCommand request,
        CancellationToken ct)
    {
        throw new NotImplementedException("BÀI TẬP 17: Implement CreateCategoryCommandHandler");
    }
}

// ╔══════════════════════════════════════════════════╗
// ║            UPDATE CATEGORY                      ║
// ╚══════════════════════════════════════════════════╝

public record UpdateCategoryCommand(
    Guid Id,
    string Name,
    string? Description
) : IRequest<CategoryDto>;

/// <summary>
/// [TODO - BÀI TẬP 18] Implement UpdateCategoryCommandHandler.
/// </summary>
public class UpdateCategoryCommandHandler(
    ICategoryRepository categoryRepository,
    IUnitOfWork unitOfWork
) : IRequestHandler<UpdateCategoryCommand, CategoryDto>
{
    public Task<CategoryDto> Handle(
        UpdateCategoryCommand request,
        CancellationToken ct)
    {
        throw new NotImplementedException("BÀI TẬP 18: Implement UpdateCategoryCommandHandler");
    }
}

public record DeleteCategoryCommand(Guid Id) : IRequest<bool>;

public class DeleteCategoryCommandHandler(
    ICategoryRepository categoryRepository,
    IUnitOfWork unitOfWork
) : IRequestHandler<DeleteCategoryCommand, bool>
{
    public Task<bool> Handle(
        DeleteCategoryCommand request,
        CancellationToken ct)
    {
        throw new NotImplementedException("BÀI TẬP 19: Implement DeleteCategoryCommandHandler");
    }
}
