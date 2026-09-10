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

        // طلبات الخياطة المربوطة بكيان الطلب (Order Entity Seed Data) - زنة يمنية مطرزة، زنه يمنيه مطرزه، بدلة رسمية كحلي فاخرة، بدلة سهرة سوداء، فستان دراعة يمنية، ثوب بشت فاخر، ثوب سعودي ملون
        modelBuilder.Entity<Order>().HasData(
            new Order
            {
                Id = 1,
                OrderNumber = "MQ-74672",

                CustomerName = "صالح الكميم",
                CustomerPhone = "0501234567",
                TailorName = "خياطة الناصر",
                ItemTitle = "زَنّة يمنية مطرزة",
                ProductImg = "/images/yemeni_zenneh.png",
                Fabric = "خياطة يمنية بالزري الفاخر",
                Date = "2026/8/20",
                ReceiptDate = "2026/8/25",
                Price = "$60",
                PaidAmount = "$60",
                Wallet = "محفظة جوالي",
                RefNo = "74672",
                Status = "جاهز للاستلام",
                CreatedAt = new DateTime(2026, 8, 20, 0, 0, 0, DateTimeKind.Utc)
            },
            new Order
            {
                Id = 2,
                OrderNumber = "MQ-41309",
                CustomerName = "صالح الكميم",
                CustomerPhone = "0501234567",
                TailorName = "الناصر للخياطة",
                ItemTitle = "بدلة رسمية كحلي فاخرة",
                ProductImg = "/images/suit_navy.jpg",
                Fabric = "صوف إيطالي كحلي عالي الجودة",
                Date = "2026/8/22",
                ReceiptDate = "2026/8/28",
                Price = "$120",
                PaidAmount = "$120",
                Wallet = "محفظة جيب",
                RefNo = "41309",
                Status = "قيد التفصيل",
                CreatedAt = new DateTime(2026, 8, 22, 0, 0, 0, DateTimeKind.Utc)
            },
            new Order
            {
                Id = 3,
                OrderNumber = "MQ-7982",
                CustomerName = "صالح الكميم",
                CustomerPhone = "0501234567",
                TailorName = "خياط الملكي",
                ItemTitle = "بدلة سهرة سوداء",
                ProductImg = "/images/suit_classic.jpg",
                Fabric = "توكسيدو أسود كلاسيك",
                Date = "2026/8/18",
                ReceiptDate = "2026/8/24",
                Price = "$140",
                PaidAmount = "$140",
                Wallet = "سداد كاك بنك",
                RefNo = "7982",
                Status = "جاهز للاستلام",
                CreatedAt = new DateTime(2026, 8, 18, 0, 0, 0, DateTimeKind.Utc)
            },
            new Order
            {
                Id = 4,
                OrderNumber = "MQ-50715",
                CustomerName = "صالح الكميم",
                CustomerPhone = "0501234567",
                TailorName = "خياطة دار الحرير",
                ItemTitle = "فستان درّاعة يمنية",
                ProductImg = "/images/yemeni_dress.png",
                Fabric = "حرير يمني فاخر",
                Date = "2026/8/23",
                ReceiptDate = "2026/8/29",
                Price = "$75",
                PaidAmount = "$75",
                Wallet = "محفظة وان كاش",
                RefNo = "50715",
                Status = "قيد التفصيل",
                CreatedAt = new DateTime(2026, 8, 23, 0, 0, 0, DateTimeKind.Utc)
            },
            new Order
            {
                Id = 5,
                OrderNumber = "MQ-3318",
                CustomerName = "صالح الكميم",
                CustomerPhone = "0501234567",
                TailorName = "خياطة الناصر",
                ItemTitle = "ثوب بشت فاخر",
                ProductImg = "/images/thobe_bisht.png",
                Fabric = "مشالح وبشوت ملكية بكسوة ذهبية",
                Date = "2026/8/15",
                ReceiptDate = "2026/8/21",
                Price = "$150",
                PaidAmount = "$150",
                Wallet = "تحويل إلكتروني",
                RefNo = "3318",
                Status = "جاهز للاستلام",
                CreatedAt = new DateTime(2026, 8, 15, 0, 0, 0, DateTimeKind.Utc)
            },
            new Order
            {
                Id = 6,
                OrderNumber = "MQ-101",
                CustomerName = "صالح الكميم",
                CustomerPhone = "0501234567",
                TailorName = "خياطة الناصر",
                ItemTitle = "ثوب سعودي ملون",
                ProductImg = "/images/thobe_saudi.png",
                Fabric = "قطن أبيض ياباني (رقم 12)",
                Date = "2026/8/23",
                ReceiptDate = "2026/8/25",
                Price = "$35",
                PaidAmount = "$35",
                Wallet = "تحويل إلكتروني",
                RefNo = "1000",
                Status = "قيد التفصيل",
                CreatedAt = new DateTime(2026, 8, 23, 0, 0, 0, DateTimeKind.Utc)
            }
        );



        modelBuilder.Entity<Product>().HasData(
            // أثواب رجالية
            new Product { Id = 1, Name = "ثوب سعودي ملون", Description = "أثواب سعودية ملونة فاخرة بحشوة راقية", Price = 35.00m, StockQuantity = 50, Category = "ثوب رجالي", ImageUrl = "/images/thobe_saudi.png", IsActive = true, CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Product { Id = 2, Name = "ثوب قطري مطرز", Description = "تفصيل قطري مع ياقة رسمية مطرزة", Price = 40.00m, StockQuantity = 40, Category = "ثوب رجالي", ImageUrl = "/images/thobe_qatari.png", IsActive = true, CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Product { Id = 3, Name = "ثوب إماراتي فاخر", Description = "أثواب إماراتية كلاسيكية بطربوشة أنيقة", Price = 45.00m, StockQuantity = 35, Category = "ثوب رجالي", ImageUrl = "/images/thobe_emirati.png", IsActive = true, CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Product { Id = 4, Name = "ثوب كويتي دبل", Description = "قصة كويتية عريضة خفيفة ومريحة", Price = 40.00m, StockQuantity = 30, Category = "ثوب رجالي", ImageUrl = "/images/thobe_kuwaiti.png", IsActive = true, CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Product { Id = 5, Name = "ثوب كويتي رسمي", Description = "أقمشة ملونة ومناسبة للعمل والزي اليومي", Price = 38.00m, StockQuantity = 45, Category = "ثوب رجالي", ImageUrl = "/images/thobe_colored.png", IsActive = true, CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Product { Id = 6, Name = "بشت ملكي فاخر", Description = "مشالح وبشوت مناسبات وأعراس بكسوة ذهبية", Price = 150.00m, StockQuantity = 15, Category = "ثوب رجالي", ImageUrl = "/images/thobe_bisht.png", IsActive = true, CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },

            // بدلات رجالية رسمية
            new Product { Id = 7, Name = "بدلة رسمية كحلي فاخرة", Description = "تصميم إيطالي بقماش صوف فاخر عالي الجودة", Price = 120.00m, StockQuantity = 20, Category = "بدلة رسمية", ImageUrl = "/images/suit_navy.jpg", IsActive = true, CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Product { Id = 8, Name = "بدلة سهرة سوداء (Black Tie)", Description = "بدلات سهرة واحتفالات رسمية كلاسيكية", Price = 140.00m, StockQuantity = 15, Category = "بدلة رسمية", ImageUrl = "/images/suit_classic.jpg", IsActive = true, CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Product { Id = 9, Name = "بدلة عريس فاخرة", Description = "طقم عريس كامل بالفيست والاكسسوارات الذهبية", Price = 200.00m, StockQuantity = 10, Category = "بدلة رسمية", ImageUrl = "/images/suit_wedding.jpg", IsActive = true, CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Product { Id = 10, Name = "بليزر كاجوال عصري", Description = "جاكيتات وبليزر كاجوال للإطلالات اليومية", Price = 85.00m, StockQuantity = 25, Category = "بدلة رسمية", ImageUrl = "/images/suit_blazer.jpg", IsActive = true, CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Product { Id = 11, Name = "بدلة توكسيدو مخمل", Description = "توكسيدو فاخر بقماش مخمل لمناسبات الليل", Price = 160.00m, StockQuantity = 12, Category = "بدلة رسمية", ImageUrl = "/images/suit_tuxedo.jpg", IsActive = true, CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Product { Id = 12, Name = "قماش بدلة إيطالي", Description = "أقمشة بدلات ممتازة للتفصيل الخارجي", Price = 70.00m, StockQuantity = 50, Category = "بدلة رسمية", ImageUrl = "/images/suit_fabric.jpg", IsActive = true, CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },

            // أزياء وخياطة نسائية
            new Product { Id = 13, Name = "بالطو يمني مخصر", Description = "بوالاط راقية وسادة بتفصيل ممتاز", Price = 50.00m, StockQuantity = 30, Category = "فستان نسائي", ImageUrl = "/images/yemeni_balto.png", IsActive = true, CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Product { Id = 14, Name = "بالطو مطرز بالزري", Description = "بوالاط مناسبات وأعراس بتطريز زري فاخر", Price = 65.00m, StockQuantity = 25, Category = "فستان نسائي", ImageUrl = "/images/yemeni_balto_embroidered.png", IsActive = true, CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Product { Id = 15, Name = "جلابية يمنية مطرزة", Description = "جلابيات واستقبال وتصميم تراثي مميز", Price = 55.00m, StockQuantity = 40, Category = "فستان نسائي", ImageUrl = "/images/yemeni_jalabiya.png", IsActive = true, CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Product { Id = 16, Name = "زَنّة يمنية مطرزة", Description = "خياطة زنين يمنية ومناسبات تقليدية", Price = 60.00m, StockQuantity = 35, Category = "فستان نسائي", ImageUrl = "/images/yemeni_zenneh.png", IsActive = true, CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Product { Id = 17, Name = "فستان درّاعة يمنية", Description = "فساتين وسهرات راقية بتصاميم حديثة", Price = 75.00m, StockQuantity = 20, Category = "فستان نسائي", ImageUrl = "/images/yemeni_dress.png", IsActive = true, CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Product { Id = 18, Name = "ثوب صنعاني مطرز", Description = "أزياء شعبية وتراثية صنعانية فاخرة", Price = 90.00m, StockQuantity = 15, Category = "فستان نسائي", ImageUrl = "/images/yemeni_traditional.png", IsActive = true, CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) }
        );
    }
}
