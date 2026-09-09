using MediatR;
using Microsoft.EntityFrameworkCore;
using Maqas.Application.Common.Interfaces;
using Maqas.Application.DTOs;
using Maqas.Domain.Entities;

namespace Maqas.Application.Features.Measurements;

public record GetAllMeasurementSessionsQuery : IRequest<IEnumerable<MeasurementSessionResponseDto>>;
public record GetMeasurementSessionByIdQuery(int Id) : IRequest<MeasurementSessionResponseDto?>;
public record GetMeasurementSessionsByStatusQuery(string Status) : IRequest<IEnumerable<MeasurementSessionResponseDto>>;

public record CreateMeasurementSessionCommand(CreateMeasurementSessionDto Dto) : IRequest<MeasurementSessionResponseDto>;
public record UpdateMeasurementSessionCommand(int Id, UpdateMeasurementSessionDto Dto) : IRequest<MeasurementSessionResponseDto?>;
public record UpdateMeasurementStatusCommand(int Id, string Status) : IRequest<bool>;
public record DeleteMeasurementSessionCommand(int Id) : IRequest<bool>;

public class GetAllMeasurementSessionsQueryHandler : IRequestHandler<GetAllMeasurementSessionsQuery, IEnumerable<MeasurementSessionResponseDto>>
{
    private readonly IAppDbContext _context;

    public GetAllMeasurementSessionsQueryHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<MeasurementSessionResponseDto>> Handle(GetAllMeasurementSessionsQuery request, CancellationToken cancellationToken)
    {
        var sessions = await _context.MeasurementSessions
            .Include(s => s.Customer)
            .Include(s => s.ClothingType)
            .Include(s => s.Details)
            .ToListAsync(cancellationToken);

        return sessions.Select(s => new MeasurementSessionResponseDto(
            s.SessionId,
            s.CustomerId,
            s.Customer?.FullName ?? string.Empty,
            s.TypeId,
            s.ClothingType?.TypeName ?? string.Empty,
            s.DateMeasured,
            s.Notes,
            s.Status,
            s.Details.Select(d => new MeasurementDetailDto(d.AttributeName, d.Value, d.TypeUnit)).ToList()
        ));
    }
}

public class GetMeasurementSessionByIdQueryHandler : IRequestHandler<GetMeasurementSessionByIdQuery, MeasurementSessionResponseDto?>
{
    private readonly IAppDbContext _context;

    public GetMeasurementSessionByIdQueryHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<MeasurementSessionResponseDto?> Handle(GetMeasurementSessionByIdQuery request, CancellationToken cancellationToken)
    {
        var session = await _context.MeasurementSessions
            .Include(s => s.Customer)
            .Include(s => s.ClothingType)
            .Include(s => s.Details)
            .FirstOrDefaultAsync(s => s.SessionId == request.Id, cancellationToken);

        if (session == null) return null;

        return new MeasurementSessionResponseDto(
            session.SessionId,
            session.CustomerId,
            session.Customer?.FullName ?? string.Empty,
            session.TypeId,
            session.ClothingType?.TypeName ?? string.Empty,
            session.DateMeasured,
            session.Notes,
            session.Status,
            session.Details.Select(d => new MeasurementDetailDto(d.AttributeName, d.Value, d.TypeUnit)).ToList()
        );
    }
}

public class GetMeasurementSessionsByStatusQueryHandler : IRequestHandler<GetMeasurementSessionsByStatusQuery, IEnumerable<MeasurementSessionResponseDto>>
{
    private readonly IAppDbContext _context;

    public GetMeasurementSessionsByStatusQueryHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<MeasurementSessionResponseDto>> Handle(GetMeasurementSessionsByStatusQuery request, CancellationToken cancellationToken)
    {
        var sessions = await _context.MeasurementSessions
            .Include(s => s.Customer)
            .Include(s => s.ClothingType)
            .Include(s => s.Details)
            .Where(s => s.Status == request.Status)
            .ToListAsync(cancellationToken);

        return sessions.Select(s => new MeasurementSessionResponseDto(
            s.SessionId,
            s.CustomerId,
            s.Customer?.FullName ?? string.Empty,
            s.TypeId,
            s.ClothingType?.TypeName ?? string.Empty,
            s.DateMeasured,
            s.Notes,
            s.Status,
            s.Details.Select(d => new MeasurementDetailDto(d.AttributeName, d.Value, d.TypeUnit)).ToList()
        ));
    }
}

public class CreateMeasurementSessionCommandHandler : IRequestHandler<CreateMeasurementSessionCommand, MeasurementSessionResponseDto>
{
    private readonly IAppDbContext _context;

    public CreateMeasurementSessionCommandHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<MeasurementSessionResponseDto> Handle(CreateMeasurementSessionCommand request, CancellationToken cancellationToken)
    {
        var customer = await _context.Customers.FindAsync(new object[] { request.Dto.CustomerId }, cancellationToken);
        if (customer == null) throw new Exception("الزبون غير موجود.");

        var clothingType = await _context.ClothingTypes.FindAsync(new object[] { request.Dto.TypeId }, cancellationToken);
        if (clothingType == null) throw new Exception("نوع الملابس غير موجود.");

        var session = new MeasurementSession
        {
            CustomerId = request.Dto.CustomerId,
            TypeId = request.Dto.TypeId,
            Notes = request.Dto.Notes,
            DateMeasured = DateTime.UtcNow,
            Status = "مقاسات جديدة"
        };

        foreach (var detailDto in request.Dto.Details)
        {
            session.Details.Add(new MeasurementDetail
            {
                AttributeName = detailDto.AttributeName,
                Value = detailDto.Value,
                TypeUnit = detailDto.TypeUnit
            });
        }

        _context.MeasurementSessions.Add(session);
        await _context.SaveChangesAsync(cancellationToken);

        return new MeasurementSessionResponseDto(
            session.SessionId,
            session.CustomerId,
            customer.FullName,
            session.TypeId,
            clothingType.TypeName,
            session.DateMeasured,
            session.Notes,
            session.Status,
            session.Details.Select(d => new MeasurementDetailDto(d.AttributeName, d.Value, d.TypeUnit)).ToList()
        );
    }
}

public class UpdateMeasurementSessionCommandHandler : IRequestHandler<UpdateMeasurementSessionCommand, MeasurementSessionResponseDto?>
{
    private readonly IAppDbContext _context;

    public UpdateMeasurementSessionCommandHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<MeasurementSessionResponseDto?> Handle(UpdateMeasurementSessionCommand request, CancellationToken cancellationToken)
    {
        var session = await _context.MeasurementSessions
            .Include(s => s.Details)
            .FirstOrDefaultAsync(s => s.SessionId == request.Id, cancellationToken);

        if (session == null) return null;

        var customer = await _context.Customers.FindAsync(new object[] { request.Dto.CustomerId }, cancellationToken);
        if (customer == null) throw new Exception("الزبون غير موجود.");

        var clothingType = await _context.ClothingTypes.FindAsync(new object[] { request.Dto.TypeId }, cancellationToken);
        if (clothingType == null) throw new Exception("نوع الملابس غير موجود.");

        session.CustomerId = request.Dto.CustomerId;
        session.TypeId = request.Dto.TypeId;
        session.Notes = request.Dto.Notes;
        session.Status = request.Dto.Status;

        session.Details.Clear();
        foreach (var detailDto in request.Dto.Details)
        {
            session.Details.Add(new MeasurementDetail
            {
                AttributeName = detailDto.AttributeName,
                Value = detailDto.Value,
                TypeUnit = detailDto.TypeUnit
            });
        }

        await _context.SaveChangesAsync(cancellationToken);

        return new MeasurementSessionResponseDto(
            session.SessionId,
            session.CustomerId,
            customer.FullName,
            session.TypeId,
            clothingType.TypeName,
            session.DateMeasured,
            session.Notes,
            session.Status,
            session.Details.Select(d => new MeasurementDetailDto(d.AttributeName, d.Value, d.TypeUnit)).ToList()
        );
    }
}

public class UpdateMeasurementStatusCommandHandler : IRequestHandler<UpdateMeasurementStatusCommand, bool>
{
    private readonly IAppDbContext _context;

    public UpdateMeasurementStatusCommandHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(UpdateMeasurementStatusCommand request, CancellationToken cancellationToken)
    {
        var session = await _context.MeasurementSessions.FindAsync(new object[] { request.Id }, cancellationToken);
        if (session == null) return false;

        session.Status = request.Status;
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}

public class DeleteMeasurementSessionCommandHandler : IRequestHandler<DeleteMeasurementSessionCommand, bool>
{
    private readonly IAppDbContext _context;

    public DeleteMeasurementSessionCommandHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(DeleteMeasurementSessionCommand request, CancellationToken cancellationToken)
    {
        var session = await _context.MeasurementSessions
            .Include(s => s.Details)
            .FirstOrDefaultAsync(s => s.SessionId == request.Id, cancellationToken);

        if (session == null) return false;

        _context.MeasurementSessions.Remove(session);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
