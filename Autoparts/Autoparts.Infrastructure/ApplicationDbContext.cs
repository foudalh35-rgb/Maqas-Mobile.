using Autoparts.Application.Interfaces;
using Autoparts.Domain;
using Microsoft.EntityFrameworkCore;

namespace Autoparts.Infrastructure;

public class ApplicationDbContext : DbContext, IApplicationDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<MaqasOrder> MaqasOrders { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Ensure decimal configuration inside DbContext for EF Core
        modelBuilder.Entity<MaqasOrder>(entity =>
        {
            entity.Property(e => e.Length).HasColumnType("decimal(18,2)");
            entity.Property(e => e.Shoulder).HasColumnType("decimal(18,2)");
            entity.Property(e => e.Waist).HasColumnType("decimal(18,2)");
            entity.Property(e => e.Neck).HasColumnType("decimal(18,2)");
            entity.Property(e => e.Arm).HasColumnType("decimal(18,2)");
            entity.Property(e => e.TotalPrice).HasColumnType("decimal(18,2)");
            entity.Property(e => e.AdvancePayment).HasColumnType("decimal(18,2)");
            entity.Property(e => e.Paid).HasColumnType("decimal(18,2)");
        });
    }
}
