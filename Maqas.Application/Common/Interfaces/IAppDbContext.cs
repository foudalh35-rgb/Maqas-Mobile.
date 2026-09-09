using Microsoft.EntityFrameworkCore;
using Maqas.Domain.Entities;

namespace Maqas.Application.Common.Interfaces;

public interface IAppDbContext
{
    DbSet<User> Users { get; }
    DbSet<Customer> Customers { get; }
    DbSet<ClothingType> ClothingTypes { get; }
    DbSet<MeasurementSession> MeasurementSessions { get; }
    DbSet<MeasurementDetail> MeasurementDetails { get; }
    DbSet<ChatMessage> ChatMessages { get; }
    DbSet<Order> Orders { get; }
    DbSet<Product> Products { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
