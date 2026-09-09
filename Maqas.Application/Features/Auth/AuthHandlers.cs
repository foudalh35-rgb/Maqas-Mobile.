using MediatR;
using Microsoft.EntityFrameworkCore;
using Maqas.Application.Common.Interfaces;
using Maqas.Application.DTOs;
using Maqas.Domain.Entities;

namespace Maqas.Application.Features.Auth;

public record LoginUserQuery(string Email, string Password, string Role) : IRequest<LoginResponseDto>;
public record RegisterUserCommand(string Name, string Email, string Password) : IRequest<UserResponseDto>;

public class LoginUserQueryHandler : IRequestHandler<LoginUserQuery, LoginResponseDto>
{
    private readonly IAppDbContext _context;

    public LoginUserQueryHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<LoginResponseDto> Handle(LoginUserQuery request, CancellationToken cancellationToken)
    {
        // التحقق من الحساب التجريبي المبدئي أو البحث في قاعدة البيانات
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == request.Email && u.Password == request.Password, cancellationToken);

        if (user != null || (request.Email == "fuad1234df@gmail.com" && request.Password == "12345678"))
        {
            var userDto = user != null ? new UserResponseDto(user.UserId, user.Name, user.Email) 
                : new UserResponseDto(1, "فؤاد علي", request.Email);

            return new LoginResponseDto(true, "تم تسجيل الدخول بنجاح", request.Role, userDto);
        }

        return new LoginResponseDto(false, "اسم المستخدم أو كلمة المرور غير صحيحة", request.Role);
    }
}

public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, UserResponseDto>
{
    private readonly IAppDbContext _context;

    public RegisterUserCommandHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<UserResponseDto> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email, cancellationToken);
        if (existingUser != null)
        {
            throw new Exception("البريد الإلكتروني مستخدم بالفعل.");
        }

        var user = new User
        {
            Name = request.Name,
            Email = request.Email,
            Password = request.Password
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync(cancellationToken);

        return new UserResponseDto(user.UserId, user.Name, user.Email);
    }
}
