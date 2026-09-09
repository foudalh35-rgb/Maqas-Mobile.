using MediatR;
using Microsoft.AspNetCore.Mvc;
using Maqas.Application.DTOs;
using Maqas.Application.Features.Customers;

namespace WebApplication1.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CustomersController : ControllerBase
{
    private readonly IMediator _mediator;

    public CustomersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CustomerResponseDto>>> GetCustomers()
    {
        var customers = await _mediator.Send(new GetCustomersQuery());
        return Ok(customers);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CustomerResponseDto>> GetCustomerById(int id)
    {
        var customer = await _mediator.Send(new GetCustomerByIdQuery(id));
        if (customer == null) return NotFound("لم يتم العثور على الزبون.");
        return Ok(customer);
    }

    [HttpPost]
    public async Task<ActionResult<CustomerResponseDto>> CreateCustomer([FromBody] CreateCustomerDto dto)
    {
        var customer = await _mediator.Send(new CreateCustomerCommand(dto));
        return CreatedAtAction(nameof(GetCustomerById), new { id = customer.CustomerId }, customer);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<CustomerResponseDto>> UpdateCustomer(int id, [FromBody] UpdateCustomerDto dto)
    {
        var customer = await _mediator.Send(new UpdateCustomerCommand(id, dto));
        if (customer == null) return NotFound("الزبون غير موجود.");
        return Ok(customer);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<string>> DeleteCustomer(int id)
    {
        var result = await _mediator.Send(new DeleteCustomerCommand(id));
        if (!result) return NotFound("الزبون غير موجود.");
        return Ok("تم حذف الزبون بنجاح.");
    }
}
