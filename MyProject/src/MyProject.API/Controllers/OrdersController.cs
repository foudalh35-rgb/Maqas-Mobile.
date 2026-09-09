using Microsoft.AspNetCore.Mvc;

namespace MyProject.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    [HttpGet]
    public IActionResult GetAll()
    {
        // TODO: Implement with MediatR/Repository
        return Ok(new { Message = "Orders endpoint ready" });
    }

    [HttpGet("{id}")]
    public IActionResult GetById(int id)
    {
        // TODO: Implement
        return Ok(new { Id = id, Message = "Order endpoint ready" });
    }

    [HttpPost]
    public IActionResult Create()
    {
        // TODO: Implement
        return Ok(new { Message = "Create order endpoint ready" });
    }
}
