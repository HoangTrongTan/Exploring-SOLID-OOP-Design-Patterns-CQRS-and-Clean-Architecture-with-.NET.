namespace CrudPractice.Application.DTOs;

/// <summary>
/// [Design Pattern - DTO (Data Transfer Object)]
/// DTO chỉ là "túi chứa data" để transfer giữa các layer.
/// KHÔNG có business logic, KHÔNG có validation trong DTO.
///
/// [SOLID - Single Responsibility]
/// ProductDto chỉ chịu trách nhiệm carry data, không làm gì khác.
///
/// [Interview term] "DTO" hay "Data Transfer Object" hay "View Model" (trong MVC)
/// hay "Response Model".
///
/// Tại sao không trả Entity trực tiếp ra API?
///   1. Security: Entity có thể có private data không muốn expose
///   2. Versioning: DTO có thể thay đổi format mà không ảnh hưởng Entity
///   3. Circular reference: Entity có navigation → serialize bị lặp vô hạn
/// </summary>
public record ProductDto(
    Guid Id,
    string Name,
    string? Description,
    decimal Price,
    int StockQuantity,
    bool IsActive,
    Guid CategoryId,
    string CategoryName,
    DateTime CreatedAt,
    DateTime? UpdatedAt
);

public record ProductSummaryDto(
    Guid Id,
    string Name,
    decimal Price,
    int StockQuantity,
    bool IsActive,
    string CategoryName
);

public record CategoryDto(
    Guid Id,
    string Name,
    string? Description,
    bool IsActive,
    DateTime CreatedAt
);

/// <summary>
/// [Design Pattern - Pagination Envelope]
/// Wrapper cho paginated results → FE biết totalCount để render pagination UI.
/// </summary>
public record PagedResult<T>(
    IEnumerable<T> Items,
    int TotalCount,
    int PageNumber,
    int PageSize
)
{
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
    public bool HasPreviousPage => PageNumber > 1;
    public bool HasNextPage => PageNumber < TotalPages;
}

/// <summary>
/// [Design Pattern - Response Envelope]
/// Nhất quán format response cho toàn bộ API.
/// FE luôn nhận được: { success, data, message, errors }
///
/// [Interview term] "API Response Wrapper" hay "Envelope Pattern"
/// </summary>
public record ApiResponse<T>
{
    public bool Success { get; init; }
    public T? Data { get; init; }
    public string? Message { get; init; }
    public IEnumerable<string> Errors { get; init; } = [];

    public static ApiResponse<T> Ok(T data, string? message = null)
        => new() { Success = true, Data = data, Message = message };

    public static ApiResponse<T> Fail(string message, IEnumerable<string>? errors = null)
        => new() { Success = false, Message = message, Errors = errors ?? [] };
}
