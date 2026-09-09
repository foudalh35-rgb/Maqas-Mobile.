using Microsoft.EntityFrameworkCore;
using Maqas.Application.Common.Interfaces;
using Maqas.Domain.Entities;

namespace Maqas.Infrastructure.Persistence;

/// <summary>
/// سياق قاعدة البيانات (AppDbContext) - الطبقة البنيوية Infrastructure
/// </summary>
public class AppDbContext : DbContext, IAppDbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<ClothingType> ClothingTypes => Set<ClothingType>();
    public DbSet<MeasurementSession> MeasurementSessions => Set<MeasurementSession>();
    public DbSet<MeasurementDetail> MeasurementDetails => Set<MeasurementDetail>();
    public DbSet<ChatMessage> ChatMessages => Set<ChatMessage>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<Product> Products => Set<Product>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<ClothingType>().HasKey(ct => ct.TypeId);
        modelBuilder.Entity<User>().HasKey(u => u.UserId);
        modelBuilder.Entity<Customer>().HasKey(c => c.CustomerId);
        modelBuilder.Entity<MeasurementSession>().HasKey(ms => ms.SessionId);
        modelBuilder.Entity<MeasurementDetail>().HasKey(md => md.DetailId);
        modelBuilder.Entity<ChatMessage>().HasKey(cm => cm.MessageId);
        modelBuilder.Entity<Order>().HasKey(o => o.Id);
        modelBuilder.Entity<Product>().HasKey(p => p.Id);

        modelBuilder.Entity<Customer>()
            .HasOne(c => c.User)
            .WithMany(u => u.Customers)
            .HasForeignKey(c => c.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<MeasurementSession>()
            .HasOne(ms => ms.Customer)
            .WithMany(c => c.MeasurementSessions)
            .HasForeignKey(ms => ms.CustomerId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<MeasurementSession>()
            .HasOne(ms => ms.ClothingType)
            .WithMany(ct => ct.MeasurementSessions)
            .HasForeignKey(ms => ms.TypeId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<MeasurementDetail>()
            .HasOne(md => md.Session)
            .WithMany(ms => ms.Details)
            .HasForeignKey(md => md.SessionId)
            .OnDelete(DeleteBehavior.Cascade);

        // البيانات الأولية (Seed Data)
        modelBuilder.Entity<ClothingType>().HasData(
            new ClothingType { TypeId = 1, TypeName = "ثوب رجالي" },
            new ClothingType { TypeId = 2, TypeName = "فستان نسائي" },
            new ClothingType { TypeId = 3, TypeName = "بدلة رسمية" },
            new ClothingType { TypeId = 4, TypeName = "قميص وبنطال" }
        );

        modelBuilder.Entity<User>().HasData(
            new User { UserId = 1, Name = "مدير الخياطة", Email = "admin@tailor.com", Password = "123" }
        );

        modelBuilder.Entity<Customer>().HasData(
            new Customer { CustomerId = 1, FullName = "أحمد علي", Phone = "0501234567", Notes = "يفضل قماش مطاطي", UserId = 1 }
        );



        modelBuilder.Entity<Product>().HasData(
            new Product
            {
                Id = 1,
                Name = "ثوب سعودي مطرز فاخر",
                Description = "قماش ياباني أبيض ممتاز خفيف ومريح",
                Price = 35.00m,
                StockQuantity = 50,
                Category = "ثوب رجالي",
                ImageUrl = "/images/thobe_saudi.png",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            },
            new Product
            {
                Id = 2,
                Name = "بدلة رسمية كلاسيكية",
                Description = "بدلة رسمية رمادي داكن صوف إيطالي",
                Price = 120.00m,
                StockQuantity = 20,
                Category = "بدلة رسمية",
                ImageUrl = "/images/suit_classic.png",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            }
        );
    }
}
