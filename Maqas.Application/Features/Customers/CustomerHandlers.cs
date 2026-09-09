using MediatR;
using Microsoft.EntityFrameworkCore;
using Maqas.Application.Common.Interfaces;
using Maqas.Application.DTOs;
using Maqas.Domain.Entities;

namespace Maqas.Application.Features.Customers;

public record GetCustomersQuery : IRequest<IEnumerable<CustomerResponseDto>>;
public record GetCustomerByIdQuery(int Id) : IRequest<CustomerResponseDto?>;
public record CreateCustomerCommand(CreateCustomerDto Dto) : IRequest<CustomerResponseDto>;
public record UpdateCustomerCommand(int Id, UpdateCustomerDto Dto) : IRequest<CustomerResponseDto?>;
public record DeleteCustomerCommand(int Id) : IRequest<bool>;

public class GetCustomersQueryHandler : IRequestHandler<GetCustomersQuery, IEnumerable<CustomerResponseDto>>
{
    private readonly IAppDbContext _context;

    public GetCustomersQueryHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<CustomerResponseDto>> Handle(GetCustomersQuery request, CancellationToken cancellationToken)
    {
        return await _context.Customers
            .Select(c => new CustomerResponseDto(c.CustomerId, c.FullName, c.Phone, c.Notes, c.UserId))
            .ToListAsync(cancellationToken);
    }
}

public class GetCustomerByIdQueryHandler : IRequestHandler<GetCustomerByIdQuery, CustomerResponseDto?>
{
    private readonly IAppDbContext _context;

    public GetCustomerByIdQueryHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<CustomerResponseDto?> Handle(GetCustomerByIdQuery request, CancellationToken cancellationToken)
    {
        var customer = await _context.Customers.FindAsync(new object[] { request.Id }, cancellationToken);
        if (customer == null) return null;
        return new CustomerResponseDto(customer.CustomerId, customer.FullName, customer.Phone, customer.Notes, customer.UserId);
    }
}

public class CreateCustomerCommandHandler : IRequestHandler<CreateCustomerCommand, CustomerResponseDto>
{
    private readonly IAppDbContext _context;

    public CreateCustomerCommandHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<CustomerResponseDto> Handle(CreateCustomerCommand request, CancellationToken cancellationToken)
    {
        var customer = new Customer
        {
            FullName = request.Dto.FullName,
            Phone = request.Dto.Phone,
            Notes = request.Dto.Notes,
            UserId = request.Dto.UserId
        };

        _context.Customers.Add(customer);
        await _context.SaveChangesAsync(cancellationToken);

        return new CustomerResponseDto(customer.CustomerId, customer.FullName, customer.Phone, customer.Notes, customer.UserId);
    }
}

public class UpdateCustomerCommandHandler : IRequestHandler<UpdateCustomerCommand, CustomerResponseDto?>
{
    private readonly IAppDbContext _context;

    public UpdateCustomerCommandHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<CustomerResponseDto?> Handle(UpdateCustomerCommand request, CancellationToken cancellationToken)
    {
        var customer = await _context.Customers.FindAsync(new object[] { request.Id }, cancellationToken);
        if (customer == null) return null;

        customer.FullName = request.Dto.FullName;
        customer.Phone = request.Dto.Phone;
        customer.Notes = request.Dto.Notes;
        customer.UserId = request.Dto.UserId;

        await _context.SaveChangesAsync(cancellationToken);
        return new CustomerResponseDto(customer.CustomerId, customer.FullName, customer.Phone, customer.Notes, customer.UserId);
    }
}

public class DeleteCustomerCommandHandler : IRequestHandler<DeleteCustomerCommand, bool>
{
    private readonly IAppDbContext _context;

    public DeleteCustomerCommandHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(DeleteCustomerCommand request, CancellationToken cancellationToken)
    {
        var customer = await _context.Customers.FindAsync(new object[] { request.Id }, cancellationToken);
        if (customer == null) return false;

        _context.Customers.Remove(customer);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
