using MediatR;
using Microsoft.EntityFrameworkCore;
using Maqas.Application.Common.Interfaces;
using Maqas.Application.DTOs;
using Maqas.Domain.Entities;

namespace Maqas.Application.Features.Products;

public record GetProductsQuery(string? Category = null) : IRequest<IEnumerable<Product>>;
public record GetProductByIdQuery(int Id) : IRequest<Product?>;
public record CreateProductCommand(Product Product) : IRequest<Product>;
public record UpdateProductCommand(int Id, Product UpdatedProduct) : IRequest<Product?>;
public record DeleteProductCommand(int Id) : IRequest<bool>;

public class GetProductsQueryHandler : IRequestHandler<GetProductsQuery, IEnumerable<Product>>
{
    private readonly IAppDbContext _context;

    public GetProductsQueryHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Product>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Products.Where(p => p.IsActive);
        if (!string.IsNullOrWhiteSpace(request.Category))
        {
            query = query.Where(p => p.Category.Contains(request.Category));
        }
        return await query.OrderBy(p => p.Id).ToListAsync(cancellationToken);
    }
}

public class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, Product?>
{
    private readonly IAppDbContext _context;

    public GetProductByIdQueryHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<Product?> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        return await _context.Products.FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);
    }
}

public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, Product>
{
    private readonly IAppDbContext _context;

    public CreateProductCommandHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<Product> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var product = request.Product;
        product.Id = 0;
        if (product.CreatedAt == default)
        {
            product.CreatedAt = DateTime.UtcNow;
        }

        _context.Products.Add(product);
        await _context.SaveChangesAsync(cancellationToken);
        return product;
    }
}

public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, Product?>
{
    private readonly IAppDbContext _context;

    public UpdateProductCommandHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<Product?> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);
        if (product == null) return null;

        var updated = request.UpdatedProduct;
        product.Name = updated.Name ?? product.Name;
        product.Description = updated.Description ?? product.Description;
        product.Price = updated.Price != 0 ? updated.Price : product.Price;
        product.StockQuantity = updated.StockQuantity != 0 ? updated.StockQuantity : product.StockQuantity;
        product.Category = updated.Category ?? product.Category;
        product.ImageUrl = updated.ImageUrl ?? product.ImageUrl;
        product.IsActive = updated.IsActive;

        await _context.SaveChangesAsync(cancellationToken);
        return product;
    }
}

public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand, bool>
{
    private readonly IAppDbContext _context;

    public DeleteProductCommandHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);
        if (product == null) return false;

        _context.Products.Remove(product);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
