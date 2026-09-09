using MediatR;
using Microsoft.AspNetCore.Mvc;
using Maqas.Application.DTOs;
using Maqas.Application.Features.ClothingTypes;

namespace WebApplication1.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ClothingTypesController : ControllerBase
{
    private readonly IMediator _mediator;

    public ClothingTypesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ClothingTypeResponseDto>>> GetClothingTypes()
    {
        var types = await _mediator.Send(new GetClothingTypesQuery());
        return Ok(types);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ClothingTypeResponseDto>> GetClothingTypeById(int id)
    {
        var type = await _mediator.Send(new GetClothingTypeByIdQuery(id));
        if (type == null) return NotFound("نوع الملابس غير موجود.");
        return Ok(type);
    }

    [HttpPost]
    public async Task<ActionResult<ClothingTypeResponseDto>> CreateClothingType([FromBody] CreateClothingTypeDto dto)
    {
        var type = await _mediator.Send(new CreateClothingTypeCommand(dto));
        return Ok(type);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ClothingTypeResponseDto>> UpdateClothingType(int id, [FromBody] UpdateClothingTypeDto dto)
    {
        var type = await _mediator.Send(new UpdateClothingTypeCommand(id, dto));
        if (type == null) return NotFound("نوع الملابس غير موجود.");
        return Ok(type);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<string>> DeleteClothingType(int id)
    {
        var result = await _mediator.Send(new DeleteClothingTypeCommand(id));
        if (!result) return NotFound("نوع الملابس غير موجود.");
        return Ok("تم حذف نوع الملابس بنجاح.");
    }
}
