using CrudPractice.Application.DTOs;
using CrudPractice.Application.Features.Products.Commands;
using CrudPractice.Application.Features.Products.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CrudPractice.API.Controllers;

/// <summary>
/// [SOLID - Single Responsibility] Controller chỉ:
///   1. Parse HTTP request
///   2. Map sang Command/Query
///   3. Dispatch qua MediatR (ISender)
///   4. Format HTTP response
/// Không có business logic ở đây.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class ProductsController(ISender mediator) : ControllerBase
{
    // GET /api/products?pageNumber=1&pageSize=10
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<ProductSummaryDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] Guid? categoryId = null,
        CancellationToken ct = default)
    {
        var result = await mediator.Send(new GetProductsQuery(pageNumber, pageSize, categoryId), ct);
        return Ok(ApiResponse<PagedResult<ProductSummaryDto>>.Ok(result));
    }

    // GET /api/products/{id}
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<ProductDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var result = await mediator.Send(new GetProductByIdQuery(id), ct);
        return Ok(ApiResponse<ProductDto>.Ok(result));
    }

    // POST /api/products
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<ProductDto>), StatusCodes.Status201Created)]
    public async Task<IActionResult> Create(
        [FromBody] CreateProductCommand command,
        CancellationToken ct)
    {
        var result = await mediator.Send(command, ct);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, ApiResponse<ProductDto>.Ok(result));
    }

    // PUT /api/products/{id}
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<ProductDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdateProductCommand command,
        CancellationToken ct)
    {
        var result = await mediator.Send(command with { Id = id }, ct);
        return Ok(ApiResponse<ProductDto>.Ok(result));
    }

    // DELETE /api/products/{id}
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await mediator.Send(new DeleteProductCommand(id), ct);
        return NoContent();
    }

    // PATCH /api/products/{id}/stock
    [HttpPatch("{id:guid}/stock")]
    [ProducesResponseType(typeof(ApiResponse<ProductDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> AdjustStock(
        Guid id,
        [FromBody] AdjustStockCommand command,
        CancellationToken ct)
    {
        var result = await mediator.Send(command with { ProductId = id }, ct);
        return Ok(ApiResponse<ProductDto>.Ok(result));
    }
}
