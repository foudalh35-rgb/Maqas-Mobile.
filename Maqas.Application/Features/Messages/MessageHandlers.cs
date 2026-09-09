using MediatR;
using Microsoft.EntityFrameworkCore;
using Maqas.Application.Common.Interfaces;
using Maqas.Application.DTOs;
using Maqas.Domain.Entities;

namespace Maqas.Application.Features.Messages;

public record GetChatMessagesQuery(int CustomerId, int TailorId, int? SessionId = null) : IRequest<IEnumerable<ChatMessageResponseDto>>;
public record SendMessageCommand(SendMessageDto Dto) : IRequest<ChatMessageResponseDto>;
public record MarkMessageAsReadCommand(int MessageId) : IRequest<bool>;

public class GetChatMessagesQueryHandler : IRequestHandler<GetChatMessagesQuery, IEnumerable<ChatMessageResponseDto>>
{
    private readonly IAppDbContext _context;

    public GetChatMessagesQueryHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<ChatMessageResponseDto>> Handle(GetChatMessagesQuery request, CancellationToken cancellationToken)
    {
        var query = _context.ChatMessages.AsQueryable();

        if (request.SessionId.HasValue && request.SessionId.Value > 0)
        {
            query = query.Where(m => m.SessionId == request.SessionId.Value);
        }
        else
        {
            query = query.Where(m =>
                (m.SenderId == request.CustomerId && m.ReceiverId == request.TailorId) ||
                (m.SenderId == request.TailorId && m.ReceiverId == request.CustomerId));
        }

        var messages = await query
            .OrderBy(m => m.SentAt)
            .Select(m => new ChatMessageResponseDto(
                m.MessageId,
                m.SenderId,
                m.SenderRole,
                m.ReceiverId,
                m.SessionId,
                m.MessageText,
                m.SentAt,
                m.IsRead
            ))
            .ToListAsync(cancellationToken);

        return messages;
    }
}

public class SendMessageCommandHandler : IRequestHandler<SendMessageCommand, ChatMessageResponseDto>
{
    private readonly IAppDbContext _context;

    public SendMessageCommandHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<ChatMessageResponseDto> Handle(SendMessageCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Dto.MessageText))
        {
            throw new Exception("نص الرسالة لا يمكن أن يكون فارغاً.");
        }

        var message = new ChatMessage
        {
            SenderId = request.Dto.SenderId,
            SenderRole = request.Dto.SenderRole,
            ReceiverId = request.Dto.ReceiverId,
            SessionId = request.Dto.SessionId,
            MessageText = request.Dto.MessageText,
            SentAt = DateTime.UtcNow,
            IsRead = false
        };

        _context.ChatMessages.Add(message);
        await _context.SaveChangesAsync(cancellationToken);

        return new ChatMessageResponseDto(
            message.MessageId,
            message.SenderId,
            message.SenderRole,
            message.ReceiverId,
            message.SessionId,
            message.MessageText,
            message.SentAt,
            message.IsRead
        );
    }
}

public class MarkMessageAsReadCommandHandler : IRequestHandler<MarkMessageAsReadCommand, bool>
{
    private readonly IAppDbContext _context;

    public MarkMessageAsReadCommandHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(MarkMessageAsReadCommand request, CancellationToken cancellationToken)
    {
        var message = await _context.ChatMessages.FindAsync(new object[] { request.MessageId }, cancellationToken);
        if (message == null) return false;

        message.IsRead = true;
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
