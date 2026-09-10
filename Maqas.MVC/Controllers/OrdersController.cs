using MediatR;
using Microsoft.AspNetCore.Mvc;
using Maqas.Application.Features.Orders;

namespace Maqas.MVC.Controllers;

public class OrdersController : Controller
{
    private readonly IMediator _mediator;

    public OrdersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task<IActionResult> Index()
    {
        var orders = await _mediator.Send(new GetOrdersQuery());
        return View(orders);
    }
}
