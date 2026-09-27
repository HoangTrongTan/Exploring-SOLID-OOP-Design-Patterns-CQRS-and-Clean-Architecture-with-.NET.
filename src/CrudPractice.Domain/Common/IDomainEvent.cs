namespace CrudPractice.Domain.Common;

/// <summary>
/// [SOLID - Interface Segregation Principle]
/// Interface nhỏ, chỉ đánh dấu "đây là domain event".
/// Không có method nào → Marker Interface pattern.
///
/// [Design Pattern - Observer / Domain Events]
/// Dùng để decouple các Aggregate Root với nhau.
/// Ví dụ: khi Product được tạo → raise ProductCreatedEvent
///        → Handler gửi email, log audit, cập nhật cache...
///        → Product không biết gì về các handler đó.
///
/// [Interview term] "Marker interface" hay "Tag interface"
/// </summary>
public interface IDomainEvent;
