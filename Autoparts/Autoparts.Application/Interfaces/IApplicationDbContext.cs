using Autoparts.Domain;
using Microsoft.EntityFrameworkCore;

namespace Autoparts.Application.Interfaces;

/// <summary>
/// واجهة DbContext تُعرّف ما تحتاجه طبقة Application من قاعدة البيانات
/// يُنفَّذ داخل Infrastructure لكنه مُعرَّف هنا لفصل الطبقات بشكل نظيف
/// </summary>
public interface IApplicationDbContext
{
    DbSet<MaqasOrder> MaqasOrders { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
