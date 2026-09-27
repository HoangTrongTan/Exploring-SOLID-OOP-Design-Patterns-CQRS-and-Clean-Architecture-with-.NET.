# 🎯 ASP.NET Interview Training — Senior TechLead Mode

> **Mục tiêu:** Master SOLID · OOP · Clean Architecture · Design Patterns · DB Optimization · ASP.NET Core internals  
> **Level:** Middle (có câu hỏi phủ xuống Junior, mở rộng lên Senior)  
> **Style:** Dùng thuật ngữ interview thực chiến — không hỏi thẳng, hỏi vòng như interview thật  
> **Chat mới:** Chỉ cần nói *"tiếp tục tech.md"* là bắt đầu ngay, không cần prompt lại

---

## 📁 Project Structure

```
Practice-01/
├── src/
│   ├── CrudPractice.API/              → Presentation layer (Controllers, Middleware)
│   │   ├── Controllers/
│   │   ├── Middleware/                → ExceptionMiddleware, RequestLoggingMiddleware
│   │   └── Program.cs
│   ├── CrudPractice.Application/     → Use Cases (CQRS: Commands + Queries)
│   │   ├── Features/
│   │   │   ├── Products/
│   │   │   │   ├── Commands/ProductCommands.cs   (Create/Update/Delete/AdjustStock)
│   │   │   │   └── Queries/ProductQueries.cs     (GetAll paginated, GetById)
│   │   │   └── Categories/
│   │   ├── Common/
│   │   │   ├── Behaviors/             → LoggingBehavior, ValidationBehavior (Pipeline)
│   │   │   └── Mappings/              → AutoMapper MappingProfile
│   │   ├── DTOs/                      → ProductDto, ProductSummaryDto, PagedResult<T>
│   │   └── DependencyInjection.cs     → AddApplication() extension method
│   ├── CrudPractice.Domain/           → Enterprise Business Rules (không phụ thuộc gì)
│   │   ├── Common/
│   │   │   ├── BaseEntity.cs          → Id, CreatedAt, UpdatedAt, DomainEvents
│   │   │   └── IDomainEvent.cs
│   │   ├── Entities/
│   │   │   ├── Product.cs             → Aggregate Root, Rich Domain Model
│   │   │   └── Category.cs            → Aggregate Root
│   │   ├── Events/DomainEvents.cs     → ProductCreatedEvent, CategoryCreatedEvent...
│   │   ├── Exceptions/                → DomainException, NotFoundException
│   │   └── Interfaces/
│   │       ├── IUnitOfWork.cs
│   │       └── Repositories/          → IProductRepository, ICategoryRepository
│   └── CrudPractice.Infrastructure/   → Data Access (EF Core + Dapper + Postgres)
│       ├── Persistence/
│       │   ├── AppDbContext.cs
│       │   ├── UnitOfWork.cs
│       │   ├── Configurations/        → Fluent API (ProductConfiguration, etc.)
│       │   └── Repositories/          → ProductRepository, CategoryRepository
│       └── DependencyInjection.cs     → AddInfrastructure() extension method
└── docker-compose.yml                 → PostgreSQL + App containers
```

---

## 🔑 Tech Stack & Key Concepts

| Thành phần | Technology | Pattern |
|---|---|---|
| Web Framework | ASP.NET Core 8 | Minimal API / MVC |
| ORM | EF Core 8 (Code First) | Repository + UoW |
| Micro-ORM | Dapper | Raw SQL reads |
| Database | PostgreSQL (via Docker) | Indexed, UTC datetime |
| Mediator | MediatR | CQRS + Pipeline Behaviors |
| Validation | FluentValidation | Declarative validation |
| Mapping | AutoMapper | DTO ↔ Entity |
| Logging | Serilog | Structured logging |
| Architecture | Clean Architecture (4 layers) | DDD-lite |

---

## 📚 Key Patterns In This Codebase

### 1. Clean Architecture Dependency Rule
```
API → Application → Domain ← Infrastructure
                  ↑
          (Infrastructure implements Domain interfaces)
```
Domain không biết gì về EF Core, HTTP, hay bất kỳ framework nào.

### 2. CQRS với MediatR
- **Command** (`IRequest<T>`): thay đổi state → dùng EF Core + UoW
- **Query** (`IRequest<T>`): chỉ đọc → dùng Dapper hoặc EF Core `AsNoTracking()`
- **Pipeline Behaviors**: `LoggingBehavior` → `ValidationBehavior` → `Handler`

### 3. Repository + Unit of Work
- Repository: ẩn DB implementation detail
- UoW: wrap nhiều repo operations trong 1 transaction
- `SaveChangesAndDispatchEventsAsync()` = save + dispatch domain events

### 4. Rich Domain Model vs Anemic Domain Model
- **Anemic**: Entity chỉ có properties, logic nằm ở Service layer ❌
- **Rich**: Entity có behavior methods (`Create()`, `UpdateInfo()`, `AddStock()`) ✅

### 5. Domain Events (Observer Pattern)
- Entity raise event (stored in `_domainEvents` list)
- UoW collect events TRƯỚC khi SaveChanges
- Sau khi save → dispatch qua MediatR `IPublisher`

---

## 🗂️ Bài Tập Tracking

| # | File | Method | Status | Chủ đề |
|---|---|---|---|---|
| 1 | `Category.cs` | `Create()` | ⬜ TODO | Factory Method, Domain Events |
| 2 | `Category.cs` | `Update()` | ⬜ TODO | Behavior Method, Encapsulation |
| 3 | `Category.cs` | `Deactivate()` | ⬜ TODO | Idempotency |
| 4 | `Product.cs` | `Create()` | ⬜ TODO | Factory Method, Validation |
| 5 | `Product.cs` | `UpdateInfo()` | ⬜ TODO | Domain Method |
| 6 | `Product.cs` | `AddStock()` | ⬜ TODO | Business Rule |
| 7 | `Product.cs` | `ReduceStock()` | ⬜ TODO | Guard Clause, Exception |
| 8 | `ProductRepository.cs` | `GetByIdAsync()` | ✅ Done (EF) | Eager Loading |
| 9 | — | — | — | — |
| 10 | `ProductQueries.cs` | `GetProductsQueryHandler` | ⬜ TODO | CQRS Query, Pagination |
| 11 | `ProductQueries.cs` | `GetProductByIdQueryHandler` | ⬜ TODO | CQRS Query, Exception |
| 12 | `ProductCommands.cs` | `CreateProductValidator` | ⬜ TODO | FluentValidation |
| 13 | `ProductCommands.cs` | `CreateProductCommandHandler` | ⬜ TODO | CQRS Command, Full flow |
| 14 | `ProductCommands.cs` | `UpdateProductCommandHandler` | ⬜ TODO | CQRS Command |
| 15 | `ProductCommands.cs` | `DeleteProductCommandHandler` | ⬜ TODO | Soft vs Hard delete |
| 16 | `ProductCommands.cs` | `AdjustStockCommandHandler` | ⬜ TODO | Domain behavior |
| 17+ | TBD | — | ⬜ TODO | Middleware, Caching, Auth... |

---

## 🎓 Interview Question Bank (Đã cover trong codebase)

### OOP
- Encapsulation, Abstraction, Inheritance, Polymorphism — nhận diện trong code thực
- `abstract class` vs `interface` — khi nào dùng cái nào?
- `protected` vs `private` setter — tại sao trong `BaseEntity`?

### SOLID
- **S** — `BaseEntity` chỉ làm 3 việc: Id, Timestamp, DomainEvents
- **O** — Thêm handler mới không sửa `Program.cs`; Behavior pipeline mở rộng được
- **L** — `IReadOnlyCollection<IDomainEvent>` là Liskov đúng nghĩa
- **I** — `IUnitOfWork` không expose `DbContext` ra ngoài
- **D** — `CreateProductCommandHandler` depend on `IProductRepository` (interface), không phải `ProductRepository` (class)

### Design Patterns
- **Factory Method**: `Category.Create()`, `Product.Create()`
- **Repository**: `IProductRepository`, `ProductRepository`
- **Unit of Work**: `IUnitOfWork`, `UnitOfWork`
- **CQRS**: Commands vs Queries
- **Pipeline (Chain of Responsibility)**: `LoggingBehavior` → `ValidationBehavior` → `Handler`
- **Observer / Domain Events**: `AddDomainEvent()` → MediatR publish
- **Facade**: `AddApplication()`, `AddInfrastructure()` extension methods

### DB & EF Core
- Eager vs Lazy vs Explicit Loading — N+1 problem
- `AsNoTracking()` — khi nào dùng, tại sao nhanh hơn?
- Offset Pagination vs Cursor/Keyset Pagination
- `decimal` vs `double` cho tiền tệ
- Code First vs Database First
- UTC DateTime convention
- Index strategy

### ASP.NET Core
- Middleware pipeline — thứ tự quan trọng
- `IServiceCollection` extension methods — DI registration
- `Scoped` vs `Transient` vs `Singleton` lifetime
- `CancellationToken` — tại sao phải pass xuống tận DB?
- Health checks, CORS, Swagger setup

---

## 📋 Session Log

| Date | Bài đã làm | Nhận xét |
|---|---|---|
| 2026-09-27 | Setup context, bắt đầu | — |

---

## 🚦 Quy trình làm việc

1. **Bạn code** → paste vào chat
2. **TechLead review** → chỉ đúng lỗi, giải thích WHY (không code thay)
3. **Nếu đúng** → tăng độ khó, hỏi thêm câu lý thuyết liên quan
4. **Nếu sai** → hint thôi, không cho đáp án ngay
5. **Sau mỗi bài** → update bảng tracking ở trên

---
# Yêu cầu tính năng: Lấy danh sách Message theo Room (CQRS Query + Read Model)

> File này là **spec/yêu cầu** — không phải code giải sẵn. Bạn tự viết code vào project theo mô tả dưới đây, chạy thử, rồi mình review lại phần bạn viết.

---

## 1. Bối cảnh / Vấn đề cần giải quyết

Hàm `GetByRoomIdAsync` hiện tại (dùng Dapper multi-mapping `<Message, User, Message>`) đang có 2 vấn đề:

1. Phải dùng **Reflection** để gán `Message.Sender` vì property này là `private set` (Domain Entity, đúng theo DDD nhưng không hợp để Dapper tự map).
2. Đang trộn lẫn **Query side** và **Command side** — trả thẳng ra `Message` (Domain Entity) thay vì tách riêng theo tinh thần **CQRS** đã học.

**Mục tiêu:** viết lại tính năng này theo đúng CQRS — tách hẳn 1 luồng đọc riêng, không đụng tới Domain Entity, không cần Reflection.

---

## 2. Việc cần làm (theo từng layer)

### a. Application layer

- Tạo 1 **Read Model** (DTO thuần, không phải Domain Entity) chứa đủ field cần hiển thị cho UI: nội dung tin nhắn, thời gian, thông tin người gửi đã "làm phẳng" (flatten) sẵn — không lồng object `User` bên trong.
- Tạo 1 **Query** (dùng MediatR `IRequest<T>`) nhận `RoomId` + tham số phân trang.
- Tạo 1 **Handler** xử lý Query đó, gọi qua 1 interface Repository **riêng cho Query** (không dùng chung interface Repository của Command side).
- Định nghĩa interface Repository riêng cho Query (đặt tên rõ ràng để phân biệt với Repository của Command, ví dụ có hậu tố `QueryRepository`).

### b. Infrastructure layer

- Implement interface Query Repository ở trên bằng Dapper, raw SQL join giữa bảng message và bảng user.
- **Không dùng multi-mapping `<T1, T2, TReturn>` nữa** — thay vào đó SELECT thẳng ra đúng tên field khớp với Read Model (1-1 mapping tự động của Dapper, không cần map thủ công, không cần Reflection).
- Đảm bảo `CancellationToken` được truyền xuyên suốt — từ chỗ mở connection **tới cả** chỗ thực thi câu query (không chỉ dừng ở việc mở connection).
- Kiểm tra lại lifetime đăng ký DI của nguồn kết nối DB (data source) — phải đảm bảo pooling hoạt động đúng, không bị tạo mới pool mỗi request.

### c. API layer

- Expose 1 endpoint GET nhận `roomId` (route) + `limit`, `offset` (query string), gọi Query qua MediatR, trả về danh sách Read Model.

---

## 3. Các trường hợp cần tự hỏi khi viết code (gợi ý, không cho sẵn đáp án)

Khi viết xong, tự kiểm tra các câu hỏi sau — đây cũng chính là các điểm mình sẽ soi kỹ lúc review:

1. Nếu client **hủy request** giữa chừng (đóng tab, timeout) thì câu SQL đang chạy dưới DB có bị hủy theo không, hay vẫn chạy tới cùng?
2. Nếu `limit` được client truyền `999999` hoặc số âm thì điều gì xảy ra? Đã chặn chưa?
3. Nếu room có **hàng trăm nghìn tin nhắn**, cách phân trang hiện tại (`OFFSET`) có còn nhanh không khi offset lớn?
4. Người gọi API có **bắt buộc phải là thành viên của room đó** thì mới xem được tin nhắn không, hay bất kỳ ai biết `roomId` cũng đọc được?
5. Nguồn kết nối DB (`NpgsqlDataSource` hoặc tương đương) đang đăng ký DI với lifetime nào — có chắc là đúng không, hay copy nhầm thói quen từ `DbContext`?
6. Read Model có vô tình lộ field nội bộ nào không nên hiển thị ra API không (ví dụ cờ `is_deleted`, id nội bộ...)?

---

## 4. Deliverable

Sau khi viết xong, nhờ review code (Read Model, Query, Handler, Repository interface + implementation, endpoint) — mình sẽ review theo đúng các chủ đề đã trao đổi (SOLID, DDD, CQRS, Repository Pattern...), chỉ ra chỗ nào viết kiểu "được nhưng nhỡ sau này X xảy ra thì sao", và gợi ý sửa.

*File này được TechLead maintain. Khi chat mới, đọc file này để lấy context.*
