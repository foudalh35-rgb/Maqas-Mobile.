using MediatR;
using Microsoft.AspNetCore.Mvc;
using Maqas.Application.Features.Orders;
using Maqas.Domain.Entities;

namespace Maqas.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IMediator _mediator;

    public OrdersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // GET: api/orders
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Order>>> GetOrders()
    {
        var orders = await _mediator.Send(new GetOrdersQuery());
        return Ok(orders);
    }

    // GET: api/orders/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Order>> GetOrder(int id)
    {
        var order = await _mediator.Send(new GetOrderByIdQuery(id));
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
        try
        {
            var created = await _mediator.Send(new CreateOrderCommand(order));
            return CreatedAtAction(nameof(GetOrder), new { id = created.Id }, created);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = "خطأ أثناء حفظ الطلب في قاعدة البيانات", details = ex.Message });
        }
    }

    // PUT: api/orders/5
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateOrder(int id, [FromBody] Order updatedOrder)
    {
        var order = await _mediator.Send(new UpdateOrderCommand(id, updatedOrder));
        if (order == null)
        {
            return NotFound(new { message = "الطلب غير موجود للتحديث." });
        }
        return Ok(order);
    }

    // DELETE: api/orders/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteOrder(int id)
    {
        var success = await _mediator.Send(new DeleteOrderCommand(id));
        if (!success)
        {
            return NotFound(new { message = "الطلب غير موجود للحذف." });
        }
        return Ok(new { message = "تم حذف الطلب بنجاح من قاعدة البيانات." });
    }

    // DELETE: api/orders/clear-all
    [HttpDelete]
    [HttpDelete("clear-all")]
    public async Task<IActionResult> ClearAllOrders()
    {
        await _mediator.Send(new ClearAllOrdersCommand());
        return Ok(new { message = "تم حذف جميع الطلبات والبيانات المحفوظة من قاعدة البيانات بنجاح." });
    }
}
