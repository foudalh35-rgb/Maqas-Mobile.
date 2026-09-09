using Autoparts.Application.DTOs;
using Autoparts.Application.Interfaces;
using Autoparts.Domain;
using Microsoft.EntityFrameworkCore;

namespace Autoparts.Application.Services;

/// <summary>
/// تنفيذ خدمة المقاسات — يحتوي على منطق الأعمال
/// ملاحظة: يستخدم IDbContextFactory أو يحقن DbContext مباشرة عبر DI
/// </summary>
public class MaqasOrderService : IMaqasOrderService
{
    private readonly IApplicationDbContext _context;

    public MaqasOrderService(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<MaqasOrderDto>> GetAllAsync()
    {
        var orders = await _context.MaqasOrders.ToListAsync();
        return orders.Select(MaqasOrderDto.FromEntity);
    }

    public async Task<MaqasOrderDto?> GetByIdAsync(int id)
    {
        var order = await _context.MaqasOrders.FindAsync(id);
        return order is null ? null : MaqasOrderDto.FromEntity(order);
    }

    public async Task<MaqasOrderDto> CreateAsync(CreateMaqasOrderDto dto)
    {
        var order = new MaqasOrder
        {
            CustomerName = dto.CustomerName,
            Phone = dto.Phone,
            FabricType = dto.FabricType,
            Length = dto.Length,
            Shoulder = dto.Shoulder,
            Waist = dto.Waist,
            Neck = dto.Neck,
            Arm = dto.Arm,
            TotalPrice = dto.TotalPrice,
            AdvancePayment = dto.AdvancePayment,
            Paid = dto.Paid,
            JobStatus = dto.JobStatus
        };

        _context.MaqasOrders.Add(order);
        await _context.SaveChangesAsync();
        return MaqasOrderDto.FromEntity(order);
    }

    public async Task<bool> UpdateStatusAsync(int id, string newStatus)
    {
        var order = await _context.MaqasOrders.FindAsync(id);
        if (order is null) return false;

        order.JobStatus = newStatus;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var order = await _context.MaqasOrders.FindAsync(id);
        if (order is null) return false;

        _context.MaqasOrders.Remove(order);
        await _context.SaveChangesAsync();
        return true;
    }
}
