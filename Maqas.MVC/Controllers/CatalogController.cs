using MediatR;
using Microsoft.AspNetCore.Mvc;
using Maqas.Application.Features.Products;

namespace Maqas.MVC.Controllers;

public class CatalogController : Controller
{
    private readonly IMediator _mediator;

    public CatalogController(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task<IActionResult> Index([FromQuery] string? category)
    {
        var products = await _mediator.Send(new GetProductsQuery(category));
        return View(products);
    }
}
