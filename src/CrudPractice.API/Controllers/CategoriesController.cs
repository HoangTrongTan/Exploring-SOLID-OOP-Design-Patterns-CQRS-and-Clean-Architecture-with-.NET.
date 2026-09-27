using CrudPractice.Application.DTOs;
using CrudPractice.Application.Features.Categories.Commands;
using CrudPractice.Application.Features.Categories.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CrudPractice.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class CategoriesController(ISender mediator) : ControllerBase
{
    // GET /api/categories
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<CategoryDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var result = await mediator.Send(new GetCategoriesQuery(), ct);
        return Ok(ApiResponse<IEnumerable<CategoryDto>>.Ok(result));
    }

    // GET /api/categories/{id}
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<CategoryDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var result = await mediator.Send(new GetCategoryByIdQuery(id), ct);
        return Ok(ApiResponse<CategoryDto>.Ok(result));
    }

    // POST /api/categories
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<CategoryDto>), StatusCodes.Status201Created)]
    public async Task<IActionResult> Create(
        [FromBody] CreateCategoryCommand command,
        CancellationToken ct)
    {
        var result = await mediator.Send(command, ct);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, ApiResponse<CategoryDto>.Ok(result));
    }

    // PUT /api/categories/{id}
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<CategoryDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdateCategoryCommand command,
        CancellationToken ct)
    {
        var result = await mediator.Send(command with { Id = id }, ct);
        return Ok(ApiResponse<CategoryDto>.Ok(result));
    }

    // DELETE /api/categories/{id}
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await mediator.Send(new DeleteCategoryCommand(id), ct);
        return NoContent();
    }
}
