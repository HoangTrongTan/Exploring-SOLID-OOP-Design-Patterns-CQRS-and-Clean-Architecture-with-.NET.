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

*File này được TechLead maintain. Khi chat mới, đọc file này để lấy context.*
