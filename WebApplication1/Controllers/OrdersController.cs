using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Maqas.Application.Common.Interfaces;
using Maqas.Domain.Entities;

namespace WebApplication1.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IAppDbContext _context;

    public OrdersController(IAppDbContext context)
    {
        _context = context;
    }

    // GET: api/orders
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Order>>> GetOrders()
    {
        var orders = await _context.Orders
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync();
        return Ok(orders);
    }

    // GET: api/orders/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Order>> GetOrder(int id)
    {
        var order = await _context.Orders.FirstOrDefaultAsync(o => o.Id == id);
        if (order == null)
        {
            return NotFound(new { message = "الطلب غير موجود." });
        }
        return Ok(order);
    }

    // POST: api/orders
    [HttpPost]
    public async Task<ActionResult<Order>> CreateOrder([FromBody] Order order)
    {
        // تصفير المعرف لمنع أي تعارض مع المفتاح التلقائي IDENTITY
        order.Id = 0;

        if (string.IsNullOrEmpty(order.OrderNumber))
        {
            order.OrderNumber = "#MQ-" + new Random().Next(1000, 9999);
        }
        if (string.IsNullOrEmpty(order.Date))
        {
            order.Date = DateTime.Now.ToString("yyyy/M/d");
        }
        if (string.IsNullOrEmpty(order.ReceiptDate))
        {
            order.ReceiptDate = DateTime.Now.AddDays(7).ToString("yyyy/M/d");
        }
        if (order.CreatedAt == default)
        {
            order.CreatedAt = DateTime.UtcNow;
        }

        try
        {
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetOrder), new { id = order.Id }, order);
        }
        catch (DbUpdateException ex)
        {
            var innerMsg = ex.InnerException?.Message ?? ex.Message;
            return BadRequest(new { message = "خطأ أثناء حفظ الطلب في قاعدة البيانات", details = innerMsg });
        }
    }

    // PUT: api/orders/5
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateOrder(int id, [FromBody] Order updatedOrder)
    {
        var order = await _context.Orders.FirstOrDefaultAsync(o => o.Id == id);
        if (order == null)
        {
            return NotFound(new { message = "الطلب غير موجود للتحديث." });
        }

        order.CustomerName = updatedOrder.CustomerName ?? order.CustomerName;
        order.CustomerPhone = updatedOrder.CustomerPhone ?? order.CustomerPhone;
        order.TailorName = updatedOrder.TailorName ?? order.TailorName;
        order.ItemTitle = updatedOrder.ItemTitle ?? order.ItemTitle;
        order.ProductImg = updatedOrder.ProductImg ?? order.ProductImg;
        order.Fabric = updatedOrder.Fabric ?? order.Fabric;
        order.Price = updatedOrder.Price ?? order.Price;
        order.PaidAmount = updatedOrder.PaidAmount ?? order.PaidAmount;
        order.Wallet = updatedOrder.Wallet ?? order.Wallet;
        order.RefNo = updatedOrder.RefNo ?? order.RefNo;
        order.Status = updatedOrder.Status ?? order.Status;

        await _context.SaveChangesAsync();
        return Ok(order);
    }

    // DELETE: api/orders/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteOrder(int id)
    {
        var order = await _context.Orders.FirstOrDefaultAsync(o => o.Id == id);
        if (order == null)
        {
            return NotFound(new { message = "الطلب غير موجود للحذف." });
        }

        _context.Orders.Remove(order);
        await _context.SaveChangesAsync();

        return Ok(new { message = "تم حذف الطلب بنجاح من قاعدة البيانات." });
    }

    // DELETE: api/orders or api/orders/clear-all
    [HttpDelete]
    [HttpDelete("clear-all")]
    public async Task<IActionResult> ClearAllOrders()
    {
        _context.Orders.RemoveRange(_context.Orders);
        await _context.SaveChangesAsync();
        return Ok(new { message = "تم حذف جميع الطلبات والبيانات المحفوظة من قاعدة البيانات بنجاح." });
    }
}
