using Autoparts.Domain.Common;
using Autoparts.Domain.Enums;

namespace Autoparts.Domain.Interfaces;

/// <summary>
/// واجهة المستودع العامة (Generic Repository Pattern)
/// تُعرّف العمليات الأساسية على مستوى Domain دون ارتباط بأي تقنية تخزين
/// </summary>
public interface IRepository<T> where T : BaseEntity
{
    Task<IEnumerable<T>> GetAllAsync();
    Task<T?> GetByIdAsync(int id);
    Task AddAsync(T entity);
    void Update(T entity);
    void Remove(T entity);
    Task<int> SaveChangesAsync();
}

/// <summary>
/// واجهة مستودع طلبات المقاسات مع عمليات خاصة
/// </summary>
public interface IMaqasOrderRepository : IRepository<MaqasOrder>
{
    /// <summary>البحث عن طلبات بحالة معينة</summary>
    Task<IEnumerable<MaqasOrder>> GetByStatusAsync(JobStatus status);

    /// <summary>البحث في طلبات عميل بعينه عبر رقم الهاتف</summary>
    Task<IEnumerable<MaqasOrder>> GetByPhoneAsync(string phone);
}
