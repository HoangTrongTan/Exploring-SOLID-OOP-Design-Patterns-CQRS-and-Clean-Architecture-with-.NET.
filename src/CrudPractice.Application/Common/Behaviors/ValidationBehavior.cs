using FluentValidation;
using MediatR;

namespace CrudPractice.Application.Common.Behaviors;

/// <summary>
/// [Design Pattern - Decorator via MediatR Pipeline]
/// ValidationBehavior tự động validate mọi request có validator tương ứng.
/// Không cần gọi validator thủ công trong từng handler.
///
/// Flow: request đến → behavior chạy → nếu có validator → validate
///   → nếu fail → throw ValidationException → ExceptionMiddleware bắt → HTTP 422
///   → nếu pass → gọi next() → Handler xử lý
///
/// [SOLID - Single Responsibility]
/// Handler chỉ lo business logic, không lo validation.
///
/// [FluentValidation] Thư viện validation phổ biến nhất cho .NET.
/// Alternative: DataAnnotations (kém powerful hơn), manual validation.
///
/// [Interview question] "FluentValidation hoạt động như thế nào trong ASP.NET?"
/// → Trả lời: Đăng ký AbstractValidator&lt;T&gt; → đặt rules trong constructor
///   → Pipeline Behavior tự động gọi validate trước khi handler chạy.
/// </summary>
public class ValidationBehavior<TRequest, TResponse>(
    IEnumerable<IValidator<TRequest>> validators)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken ct)
    {
        // Nếu không có validator nào → bỏ qua, tiếp tục pipeline
        if (!validators.Any())
            return await next();

        var context = new ValidationContext<TRequest>(request);

        // Chạy tất cả validators song song (WhenAll) → tổng hợp lỗi
        // Tốt hơn chạy tuần tự vì không cần kết quả của validator trước
        var validationResults = await Task.WhenAll(
            validators.Select(v => v.ValidateAsync(context, ct)));

        var failures = validationResults
            .SelectMany(r => r.Errors)
            .Where(f => f is not null)
            .ToList();

        if (failures.Count > 0)
            throw new ValidationException(failures);

        return await next();
    }
}
