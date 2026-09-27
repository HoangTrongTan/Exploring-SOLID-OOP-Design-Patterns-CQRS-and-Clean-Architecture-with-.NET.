namespace CrudPractice.Domain.Exceptions;

/// <summary>
/// [Design Pattern - Custom Exception]
/// DomainException là exception cho vi phạm business rule.
/// Middleware sẽ map → HTTP 400 Bad Request.
///
/// Ví dụ: Product.SetPrice(-10) → throw DomainException("Price cannot be negative")
///
/// [SOLID - Single Responsibility] Exception class này chỉ biểu diễn "lỗi domain".
/// Không biết HTTP status code → đó là việc của Middleware.
/// </summary>
public class DomainException(string message) : Exception(message);

/// <summary>
/// NotFoundException là exception khi không tìm thấy resource.
/// Middleware sẽ map → HTTP 404 Not Found.
///
/// [Interview term] "Resource not found" hay "Entity not found"
/// </summary>
public class NotFoundException(string entityName, object key)
    : Exception($"{entityName} with key '{key}' was not found.");
