using MediatR;
using Microsoft.AspNetCore.Mvc;
using Maqas.Application.Features.Products;
using Maqas.Domain.Entities;

namespace Maqas.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProductsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // GET: api/products
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Product>>> GetProducts([FromQuery] string? category = null)
    {
        var products = await _mediator.Send(new GetProductsQuery(category));
        return Ok(products);
    }

    // GET: api/products/category/ثوب%20رجالي
    [HttpGet("category/{category}")]
    public async Task<ActionResult<IEnumerable<Product>>> GetProductsByCategory(string category)
    {
        var products = await _mediator.Send(new GetProductsQuery(category));
        return Ok(products);
    }

    // GET: api/products/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Product>> GetProduct(int id)
    {
        var product = await _mediator.Send(new GetProductByIdQuery(id));
        if (product == null)
        {
            return NotFound(new { message = "المنتج غير موجود." });
        }
        return Ok(product);
    }

    // POST: api/products
    [HttpPost]
    public async Task<ActionResult<Product>> CreateProduct([FromBody] Product product)
    {
        try
        {
            var created = await _mediator.Send(new CreateProductCommand(product));
            return CreatedAtAction(nameof(GetProduct), new { id = created.Id }, created);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = "خطأ أثناء حفظ المنتج في قاعدة البيانات", details = ex.Message });
        }
    }

    // PUT: api/products/5
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProduct(int id, [FromBody] Product updatedProduct)
    {
        var product = await _mediator.Send(new UpdateProductCommand(id, updatedProduct));
        if (product == null)
        {
            return NotFound(new { message = "المنتج غير موجود للتحديث." });
        }
        return Ok(product);
    }

    // DELETE: api/products/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        var success = await _mediator.Send(new DeleteProductCommand(id));
        if (!success)
        {
            return NotFound(new { message = "المنتج غير موجود للحذف." });
        }
        return Ok(new { message = "تم حذف المنتج بنجاح من قاعدة البيانات المشتركة." });
    }
}
