using MediatR;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace CrudPractice.Application.Common.Behaviors;

/// <summary>
/// [Design Pattern - Decorator / Pipeline Behavior]
/// MediatR Pipeline Behavior tự động log mọi request/response.
/// Không cần thêm log code vào từng handler.
///
/// Pipeline thứ tự: Request → [LoggingBehavior] → [ValidationBehavior] → Handler → Response
///
/// [SOLID - Open/Closed Principle]
/// Thêm logging mà không cần sửa bất kỳ handler nào.
///
/// [SOLID - Single Responsibility]
/// Handler chỉ lo business logic, behavior lo logging.
///
/// [Interview term] "Cross-cutting Concerns" hay "AOP - Aspect Oriented Programming"
/// Logging, validation, caching là cross-cutting concerns → nên dùng behavior/middleware.
///
/// [Interview question] "Tại sao lại dùng MediatR Pipeline Behavior thay vì viết trong handler?"
/// → Trả lời: DRY principle, cross-cutting concerns, không vi phạm SRP.
/// </summary>
public class LoggingBehavior<TRequest, TResponse>(
    ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken ct)
    {
        var requestName = typeof(TRequest).Name;
        var sw = Stopwatch.StartNew();

        logger.LogInformation("[START] Handling {RequestName}", requestName);

        try
        {
            var response = await next();
            sw.Stop();

            logger.LogInformation(
                "[END] {RequestName} completed in {ElapsedMs}ms",
                requestName, sw.ElapsedMilliseconds);

            // [Performance Monitoring] Cảnh báo slow query > 500ms
            if (sw.ElapsedMilliseconds > 500)
                logger.LogWarning(
                    "[SLOW] {RequestName} took {ElapsedMs}ms — cần optimize!",
                    requestName, sw.ElapsedMilliseconds);

            return response;
        }
        catch (Exception ex)
        {
            sw.Stop();
            logger.LogError(ex, "[ERROR] {RequestName} failed after {ElapsedMs}ms",
                requestName, sw.ElapsedMilliseconds);
            throw;
        }
    }
}
