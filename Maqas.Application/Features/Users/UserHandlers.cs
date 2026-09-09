using MediatR;
using Microsoft.EntityFrameworkCore;
using Maqas.Application.Common.Interfaces;
using Maqas.Application.DTOs;
using Maqas.Domain.Entities;

namespace Maqas.Application.Features.Users;

public record GetUsersQuery : IRequest<IEnumerable<UserResponseDto>>;
public record GetUserByIdQuery(int Id) : IRequest<UserResponseDto?>;
public record CreateUserCommand(RegisterDto Dto) : IRequest<UserResponseDto>;
public record UpdateUserCommand(int Id, UpdateUserDto Dto) : IRequest<UserResponseDto?>;
public record DeleteUserCommand(int Id) : IRequest<bool>;

public class GetUsersQueryHandler : IRequestHandler<GetUsersQuery, IEnumerable<UserResponseDto>>
{
    private readonly IAppDbContext _context;

    public GetUsersQueryHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<UserResponseDto>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
    {
        return await _context.Users
            .Select(u => new UserResponseDto(u.UserId, u.Name, u.Email))
            .ToListAsync(cancellationToken);
    }
}

public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, UserResponseDto?>
{
    private readonly IAppDbContext _context;

    public GetUserByIdQueryHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<UserResponseDto?> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        var user = await _context.Users.FindAsync(new object[] { request.Id }, cancellationToken);
        if (user == null) return null;
        return new UserResponseDto(user.UserId, user.Name, user.Email);
    }
}

public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, UserResponseDto>
{
    private readonly IAppDbContext _context;

    public CreateUserCommandHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<UserResponseDto> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var user = new User
        {
            Name = request.Dto.Name,
            Email = request.Dto.Email,
            Password = request.Dto.Password
        };
        _context.Users.Add(user);
        await _context.SaveChangesAsync(cancellationToken);
        return new UserResponseDto(user.UserId, user.Name, user.Email);
    }
}

public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, UserResponseDto?>
{
    private readonly IAppDbContext _context;

    public UpdateUserCommandHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<UserResponseDto?> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _context.Users.FindAsync(new object[] { request.Id }, cancellationToken);
        if (user == null) return null;

        user.Name = request.Dto.Name;
        user.Email = request.Dto.Email;
        user.Password = request.Dto.Password;

        await _context.SaveChangesAsync(cancellationToken);
        return new UserResponseDto(user.UserId, user.Name, user.Email);
    }
}

public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, bool>
{
    private readonly IAppDbContext _context;

    public DeleteUserCommandHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _context.Users.FindAsync(new object[] { request.Id }, cancellationToken);
        if (user == null) return false;

        _context.Users.Remove(user);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
