using MediatR;
using MyProject.Application.DTOs;

namespace MyProject.Application.Features.Products.Queries.GetProducts;

public record GetProductsQuery() : IRequest<IEnumerable<ProductDto>>;
