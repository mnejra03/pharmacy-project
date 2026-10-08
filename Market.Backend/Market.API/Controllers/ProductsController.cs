using Market.Application.Modules.Catalog;

namespace Market.API.Controllers;

[ApiController, Route("api/products")]
public sealed class ProductsController(IMediator mediator) : ControllerBase
{
    [HttpGet, AllowAnonymous]
    public async Task<ActionResult<PageResult<ProductDto>>> Get([FromQuery] ProductListQuery query, CancellationToken ct) => Ok(await mediator.Send(query, ct));

    [HttpGet("{id:int}"), AllowAnonymous]
    public async Task<ActionResult<ProductDto>> GetById(int id, CancellationToken ct) => Ok(await mediator.Send(new ProductByIdQuery(id), ct));

    [HttpPost, Authorize]
    public async Task<ActionResult<ProductDto>> Create([FromBody] SaveProductCommand command, CancellationToken ct) => Ok(await mediator.Send(command with { Id = null }, ct));

    [HttpPut("{id:int}"), Authorize]
    public async Task<ActionResult<ProductDto>> Update(int id, [FromBody] SaveProductCommand command, CancellationToken ct) => Ok(await mediator.Send(command with { Id = id }, ct));

    [HttpDelete("{id:int}"), Authorize]
    public async Task<IActionResult> Delete(int id, CancellationToken ct) { await mediator.Send(new DeleteProductCommand(id), ct); return NoContent(); }
}

[ApiController, Route("api/categories")]
public sealed class CategoriesController(IMediator mediator) : ControllerBase
{
    [HttpGet, AllowAnonymous]
    public async Task<ActionResult<IReadOnlyList<CategoryDto>>> Get(CancellationToken ct) => Ok(await mediator.Send(new GetCategoriesQuery(), ct));
}

[ApiController, Route("api/brands")]
public sealed class BrandsController(IMediator mediator) : ControllerBase
{
    [HttpGet, AllowAnonymous]
    public async Task<ActionResult<IReadOnlyList<BrandDto>>> Get(CancellationToken ct) => Ok(await mediator.Send(new GetBrandsQuery(), ct));
}
