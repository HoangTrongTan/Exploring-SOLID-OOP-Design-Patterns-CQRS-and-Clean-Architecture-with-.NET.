using AutoMapper;
using CrudPractice.Application.Common.Behaviors;
using CrudPractice.Application.Common.Mappings;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace CrudPractice.Application;

/// <summary>
/// [Design Pattern - Extension Method]
/// Clean Dependency Injection registration cho Application layer.
/// Program.cs chỉ cần gọi builder.Services.AddApplication() → gọn gàng.
///
/// [SOLID - Open/Closed Principle]
/// Thêm handler/behavior/validator mới → chỉ sửa file này (hoặc assembly scan tự tìm).
/// Không cần sửa Program.cs.
///
/// [Design Pattern - Facade]
/// AddApplication() là facade ẩn đi toàn bộ complexity của DI registration.
/// Caller (Program.cs) không cần biết bên trong dùng MediatR, AutoMapper, FluentValidation.
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var assembly = typeof(DependencyInjection).Assembly;

        // [MediatR] Scan assembly → tự tìm tất cả IRequestHandler, INotificationHandler
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(assembly);

            // [Pipeline Behaviors] Thứ tự quan trọng!
            // LoggingBehavior chạy trước → wrap toàn bộ pipeline
            // ValidationBehavior chạy sau → validate trước khi vào Handler
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        });

        // [FluentValidation] Scan assembly → tự tìm tất cả AbstractValidator<T>
        services.AddValidatorsFromAssembly(assembly);

        // [AutoMapper] Scan assembly → tìm tất cả Profile (MappingProfile)
        services.AddAutoMapper(assembly);

        return services;
    }
}
