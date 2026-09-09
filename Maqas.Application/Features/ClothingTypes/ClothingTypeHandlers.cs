using MediatR;
using Microsoft.EntityFrameworkCore;
using Maqas.Application.Common.Interfaces;
using Maqas.Application.DTOs;
using Maqas.Domain.Entities;

namespace Maqas.Application.Features.ClothingTypes;

public record GetClothingTypesQuery : IRequest<IEnumerable<ClothingTypeResponseDto>>;
public record GetClothingTypeByIdQuery(int Id) : IRequest<ClothingTypeResponseDto?>;
public record CreateClothingTypeCommand(CreateClothingTypeDto Dto) : IRequest<ClothingTypeResponseDto>;
public record UpdateClothingTypeCommand(int Id, UpdateClothingTypeDto Dto) : IRequest<ClothingTypeResponseDto?>;
public record DeleteClothingTypeCommand(int Id) : IRequest<bool>;

public class GetClothingTypesQueryHandler : IRequestHandler<GetClothingTypesQuery, IEnumerable<ClothingTypeResponseDto>>
{
    private readonly IAppDbContext _context;

    public GetClothingTypesQueryHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<ClothingTypeResponseDto>> Handle(GetClothingTypesQuery request, CancellationToken cancellationToken)
    {
        return await _context.ClothingTypes
            .Select(t => new ClothingTypeResponseDto(t.TypeId, t.TypeName))
            .ToListAsync(cancellationToken);
    }
}

public class GetClothingTypeByIdQueryHandler : IRequestHandler<GetClothingTypeByIdQuery, ClothingTypeResponseDto?>
{
    private readonly IAppDbContext _context;

    public GetClothingTypeByIdQueryHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<ClothingTypeResponseDto?> Handle(GetClothingTypeByIdQuery request, CancellationToken cancellationToken)
    {
        var type = await _context.ClothingTypes.FindAsync(new object[] { request.Id }, cancellationToken);
        if (type == null) return null;
        return new ClothingTypeResponseDto(type.TypeId, type.TypeName);
    }
}

public class CreateClothingTypeCommandHandler : IRequestHandler<CreateClothingTypeCommand, ClothingTypeResponseDto>
{
    private readonly IAppDbContext _context;

    public CreateClothingTypeCommandHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<ClothingTypeResponseDto> Handle(CreateClothingTypeCommand request, CancellationToken cancellationToken)
    {
        var type = new ClothingType { TypeName = request.Dto.TypeName };
        _context.ClothingTypes.Add(type);
        await _context.SaveChangesAsync(cancellationToken);
        return new ClothingTypeResponseDto(type.TypeId, type.TypeName);
    }
}

public class UpdateClothingTypeCommandHandler : IRequestHandler<UpdateClothingTypeCommand, ClothingTypeResponseDto?>
{
    private readonly IAppDbContext _context;

    public UpdateClothingTypeCommandHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<ClothingTypeResponseDto?> Handle(UpdateClothingTypeCommand request, CancellationToken cancellationToken)
    {
        var type = await _context.ClothingTypes.FindAsync(new object[] { request.Id }, cancellationToken);
        if (type == null) return null;

        type.TypeName = request.Dto.TypeName;
        await _context.SaveChangesAsync(cancellationToken);
        return new ClothingTypeResponseDto(type.TypeId, type.TypeName);
    }
}

public class DeleteClothingTypeCommandHandler : IRequestHandler<DeleteClothingTypeCommand, bool>
{
    private readonly IAppDbContext _context;

    public DeleteClothingTypeCommandHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(DeleteClothingTypeCommand request, CancellationToken cancellationToken)
    {
        var type = await _context.ClothingTypes.FindAsync(new object[] { request.Id }, cancellationToken);
        if (type == null) return false;

        _context.ClothingTypes.Remove(type);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
