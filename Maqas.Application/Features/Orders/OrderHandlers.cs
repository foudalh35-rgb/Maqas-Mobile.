using MediatR;
using Microsoft.EntityFrameworkCore;
using Maqas.Application.Common.Interfaces;
using Maqas.Application.DTOs;
using Maqas.Domain.Entities;

namespace Maqas.Application.Features.Orders;

public record GetOrdersQuery : IRequest<IEnumerable<Order>>;
public record GetOrderByIdQuery(int Id) : IRequest<Order?>;
public record CreateOrderCommand(Order Order) : IRequest<Order>;
public record UpdateOrderCommand(int Id, Order UpdatedOrder) : IRequest<Order?>;
public record DeleteOrderCommand(int Id) : IRequest<bool>;
public record ClearAllOrdersCommand : IRequest<bool>;

public class GetOrdersQueryHandler : IRequestHandler<GetOrdersQuery, IEnumerable<Order>>
{
    private readonly IAppDbContext _context;

    public GetOrdersQueryHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Order>> Handle(GetOrdersQuery request, CancellationToken cancellationToken)
    {
        return await _context.Orders
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync(cancellationToken);
    }
}

public class GetOrderByIdQueryHandler : IRequestHandler<GetOrderByIdQuery, Order?>
{
    private readonly IAppDbContext _context;

    public GetOrderByIdQueryHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<Order?> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
    {
        return await _context.Orders.FirstOrDefaultAsync(o => o.Id == request.Id, cancellationToken);
    }
}

public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, Order>
{
    private readonly IAppDbContext _context;

    public CreateOrderCommandHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<Order> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        var order = request.Order;
        order.Id = 0;

        if (string.IsNullOrEmpty(order.OrderNumber))
        {
            order.OrderNumber = "#MQ-" + Random.Shared.Next(1000, 9999);
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

        _context.Orders.Add(order);
        await _context.SaveChangesAsync(cancellationToken);
        return order;
    }
}

public class UpdateOrderCommandHandler : IRequestHandler<UpdateOrderCommand, Order?>
{
    private readonly IAppDbContext _context;

    public UpdateOrderCommandHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<Order?> Handle(UpdateOrderCommand request, CancellationToken cancellationToken)
    {
        var order = await _context.Orders.FirstOrDefaultAsync(o => o.Id == request.Id, cancellationToken);
        if (order == null) return null;

        var updated = request.UpdatedOrder;
        order.CustomerName = updated.CustomerName ?? order.CustomerName;
        order.CustomerPhone = updated.CustomerPhone ?? order.CustomerPhone;
        order.TailorName = updated.TailorName ?? order.TailorName;
        order.ItemTitle = updated.ItemTitle ?? order.ItemTitle;
        order.ProductImg = updated.ProductImg ?? order.ProductImg;
        order.Fabric = updated.Fabric ?? order.Fabric;
        order.Price = updated.Price ?? order.Price;
        order.PaidAmount = updated.PaidAmount ?? order.PaidAmount;
        order.Wallet = updated.Wallet ?? order.Wallet;
        order.RefNo = updated.RefNo ?? order.RefNo;
        order.Status = updated.Status ?? order.Status;

        await _context.SaveChangesAsync(cancellationToken);
        return order;
    }
}

public class DeleteOrderCommandHandler : IRequestHandler<DeleteOrderCommand, bool>
{
    private readonly IAppDbContext _context;

    public DeleteOrderCommandHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(DeleteOrderCommand request, CancellationToken cancellationToken)
    {
        var order = await _context.Orders.FirstOrDefaultAsync(o => o.Id == request.Id, cancellationToken);
        if (order == null) return false;

        _context.Orders.Remove(order);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}

public class ClearAllOrdersCommandHandler : IRequestHandler<ClearAllOrdersCommand, bool>
{
    private readonly IAppDbContext _context;

    public ClearAllOrdersCommandHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(ClearAllOrdersCommand request, CancellationToken cancellationToken)
    {
        _context.Orders.RemoveRange(_context.Orders);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
