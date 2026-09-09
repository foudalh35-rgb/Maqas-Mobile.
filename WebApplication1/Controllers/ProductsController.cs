using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Maqas.Application.Common.Interfaces;
using Maqas.Domain.Entities;

namespace WebApplication1.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IAppDbContext _context;

    public ProductsController(IAppDbContext context)
    {
        _context = context;
    }

    // GET: api/products
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
    {
        var products = await _context.Products
            .Where(p => p.IsActive)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();
        return Ok(products);
    }

    // GET: api/products/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Product>> GetProduct(int id)
    {
        var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);
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
        product.Id = 0;
        if (product.CreatedAt == default)
        {
            product.CreatedAt = DateTime.UtcNow;
        }

        try
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
        }
        catch (DbUpdateException ex)
        {
            var innerMsg = ex.InnerException?.Message ?? ex.Message;
            return BadRequest(new { message = "خطأ أثناء حفظ المنتج في قاعدة البيانات", details = innerMsg });
        }
    }

    // PUT: api/products/5
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProduct(int id, [FromBody] Product updatedProduct)
    {
        var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);
        if (product == null)
        {
            return NotFound(new { message = "المنتج غير موجود للتحديث." });
        }

        product.Name = updatedProduct.Name ?? product.Name;
        product.Description = updatedProduct.Description ?? product.Description;
        product.Price = updatedProduct.Price != 0 ? updatedProduct.Price : product.Price;
        product.StockQuantity = updatedProduct.StockQuantity != 0 ? updatedProduct.StockQuantity : product.StockQuantity;
        product.Category = updatedProduct.Category ?? product.Category;
        product.ImageUrl = updatedProduct.ImageUrl ?? product.ImageUrl;
        product.IsActive = updatedProduct.IsActive;

        await _context.SaveChangesAsync();
        return Ok(product);
    }

    // DELETE: api/products/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);
        if (product == null)
        {
            return NotFound(new { message = "المنتج غير موجود للحذف." });
        }

        _context.Products.Remove(product);
        await _context.SaveChangesAsync();

        return Ok(new { message = "تم حذف المنتج بنجاح من قاعدة البيانات المشتركة." });
    }
}
