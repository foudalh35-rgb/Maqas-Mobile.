using Autoparts.Application.DTOs;
using Autoparts.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Autoparts.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MaqasController : ControllerBase
{
    private readonly IMaqasOrderService _service;

    public MaqasController(IMaqasOrderService service)
    {
        _service = service;
    }

    // GET: api/Maqas
    [HttpGet]
    public async Task<ActionResult<IEnumerable<MaqasOrderDto>>> GetAll()
    {
        var orders = await _service.GetAllAsync();
        return Ok(orders);
    }

    // GET: api/Maqas/5
    [HttpGet("{id}")]
    public async Task<ActionResult<MaqasOrderDto>> GetById(int id)
    {
        var order = await _service.GetByIdAsync(id);
        if (order is null) return NotFound();
        return Ok(order);
    }

    // POST: api/Maqas
    [HttpPost]
    public async Task<ActionResult<MaqasOrderDto>> Create(CreateMaqasOrderDto dto)
    {
        var created = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    // PATCH: api/Maqas/5/status
    [HttpPatch("{id}/status")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] string newStatus)
    {
        var result = await _service.UpdateStatusAsync(id, newStatus);
        return result ? NoContent() : NotFound();
    }

    // DELETE: api/Maqas/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _service.DeleteAsync(id);
        return result ? NoContent() : NotFound();
    }
}
