using Scalar.AspNetCore;
using System.Text.Json.Serialization;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Maqas.Infrastructure;
using Maqas.Application.Common.Interfaces;
using Maqas.Infrastructure.Persistence;
using Maqas.Domain.Entities;

var builder = WebApplication.CreateBuilder(args);

// 1. إضافة خدمة البنية التحتية (Infrastructure) وقاعدة البيانات
builder.Services.AddInfrastructure(builder.Configuration);

// 2. إضافة MediatR من طبقة Application لتشغيل نمط CQRS
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(IAppDbContext).Assembly));

// 3. إضافة المتحكمات (Controllers) وتفادي التكرار الدوري في JSON
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// 4. توثيق OpenAPI (Swagger/Scalar)
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// تهيئة قاعدة البيانات والتأكد من إنشاء الجداول المحدثة بالبيانات الأولية
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    context.Database.EnsureCreated();

    // التأكد من وجود جدول Orders إذا كانت قاعدة البيانات قديمة
    var ensureOrdersTableSql = @"
        IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Orders')
        BEGIN
            CREATE TABLE [Orders] (
                [Id] int NOT NULL IDENTITY(1,1),
                [OrderNumber] nvarchar(max) NOT NULL DEFAULT N'',
                [CustomerName] nvarchar(max) NOT NULL DEFAULT N'',
                [CustomerPhone] nvarchar(max) NOT NULL DEFAULT N'',
                [TailorName] nvarchar(max) NOT NULL DEFAULT N'',
                [ItemTitle] nvarchar(max) NOT NULL DEFAULT N'',
                [ProductImg] nvarchar(max) NOT NULL DEFAULT N'',
                [Fabric] nvarchar(max) NOT NULL DEFAULT N'',
                [Date] nvarchar(max) NOT NULL DEFAULT N'',
                [ReceiptDate] nvarchar(max) NOT NULL DEFAULT N'',
                [Price] nvarchar(max) NOT NULL DEFAULT N'',
                [PaidAmount] nvarchar(max) NOT NULL DEFAULT N'',
                [Wallet] nvarchar(max) NOT NULL DEFAULT N'',
                [RefNo] nvarchar(max) NOT NULL DEFAULT N'',
                [Status] nvarchar(max) NOT NULL DEFAULT N'قيد التفصيل',
                [CreatedAt] datetime2 NOT NULL DEFAULT GETUTCDATE(),
                CONSTRAINT [PK_Orders] PRIMARY KEY ([Id])
            );
        END

        IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Products')
        BEGIN
            CREATE TABLE [Products] (
                [Id] int NOT NULL IDENTITY(1,1),
                [Name] nvarchar(max) NOT NULL DEFAULT N'',
                [Description] nvarchar(max) NOT NULL DEFAULT N'',
                [Price] decimal(18,2) NOT NULL DEFAULT 0.0,
                [StockQuantity] int NOT NULL DEFAULT 0,
                [Category] nvarchar(max) NOT NULL DEFAULT N'',
                [ImageUrl] nvarchar(max) NOT NULL DEFAULT N'',
                [IsActive] bit NOT NULL DEFAULT 1,
                [CreatedAt] datetime2 NOT NULL DEFAULT GETUTCDATE(),
                CONSTRAINT [PK_Products] PRIMARY KEY ([Id])
            );
        END";

    try
    {
        context.Database.ExecuteSqlRaw(ensureOrdersTableSql);

        if (!context.Products.Any())
        {
            context.Products.AddRange(
                new Product { Name = "ثوب سعودي ملون", Description = "أثواب سعودية ملونة فاخرة بحشوة راقية", Price = 35.00m, StockQuantity = 50, Category = "ثوب رجالي", ImageUrl = "/images/thobe_saudi.png", IsActive = true, CreatedAt = DateTime.UtcNow },
                new Product { Name = "ثوب قطري مطرز", Description = "تفصيل قطري مع ياقة رسمية مطرزة", Price = 40.00m, StockQuantity = 40, Category = "ثوب رجالي", ImageUrl = "/images/thobe_qatari.png", IsActive = true, CreatedAt = DateTime.UtcNow },
                new Product { Name = "ثوب إماراتي فاخر", Description = "أثواب إماراتية كلاسيكية بطربوشة أنيقة", Price = 45.00m, StockQuantity = 35, Category = "ثوب رجالي", ImageUrl = "/images/thobe_emirati.png", IsActive = true, CreatedAt = DateTime.UtcNow },
                new Product { Name = "ثوب كويتي دبل", Description = "قصة كويتية عريضة خفيفة ومريحة", Price = 40.00m, StockQuantity = 30, Category = "ثوب رجالي", ImageUrl = "/images/thobe_kuwaiti.png", IsActive = true, CreatedAt = DateTime.UtcNow },
                new Product { Name = "ثوب كويتي رسمي", Description = "أقمشة ملونة ومناسبة للعمل والزي اليومي", Price = 38.00m, StockQuantity = 45, Category = "ثوب رجالي", ImageUrl = "/images/thobe_colored.png", IsActive = true, CreatedAt = DateTime.UtcNow },
                new Product { Name = "بشت ملكي فاخر", Description = "مشالح وبشوت مناسبات وأعراس بكسوة ذهبية", Price = 150.00m, StockQuantity = 15, Category = "ثوب رجالي", ImageUrl = "/images/thobe_bisht.png", IsActive = true, CreatedAt = DateTime.UtcNow },

                new Product { Name = "بدلة رسمية كحلي فاخرة", Description = "تصميم إيطالي بقماش صوف فاخر عالي الجودة", Price = 120.00m, StockQuantity = 20, Category = "بدلة رسمية", ImageUrl = "/images/suit_navy.jpg", IsActive = true, CreatedAt = DateTime.UtcNow },
                new Product { Name = "بدلة سهرة سوداء (Black Tie)", Description = "بدلات سهرة واحتفالات رسمية كلاسيكية", Price = 140.00m, StockQuantity = 15, Category = "بدلة رسمية", ImageUrl = "/images/suit_classic.jpg", IsActive = true, CreatedAt = DateTime.UtcNow },
                new Product { Name = "بدلة عريس فاخرة", Description = "طقم عريس كامل بالفيست والاكسسوارات الذهبية", Price = 200.00m, StockQuantity = 10, Category = "بدلة رسمية", ImageUrl = "/images/suit_wedding.jpg", IsActive = true, CreatedAt = DateTime.UtcNow },
                new Product { Name = "بليزر كاجوال عصري", Description = "جاكيتات وبليزر كاجوال للإطلالات اليومية", Price = 85.00m, StockQuantity = 25, Category = "بدلة رسمية", ImageUrl = "/images/suit_blazer.jpg", IsActive = true, CreatedAt = DateTime.UtcNow },
                new Product { Name = "بدلة توكسيدو مخمل", Description = "توكسيدو فاخر بقماش مخمل لمناسبات الليل", Price = 160.00m, StockQuantity = 12, Category = "بدلة رسمية", ImageUrl = "/images/suit_tuxedo.jpg", IsActive = true, CreatedAt = DateTime.UtcNow },
                new Product { Name = "قماش بدلة إيطالي", Description = "أقمشة بدلات ممتازة للتفصيل الخارجي", Price = 70.00m, StockQuantity = 50, Category = "بدلة رسمية", ImageUrl = "/images/suit_fabric.jpg", IsActive = true, CreatedAt = DateTime.UtcNow },

                new Product { Name = "بالطو يمني مخصر", Description = "بوالاط راقية وسادة بتفصيل ممتاز", Price = 50.00m, StockQuantity = 30, Category = "فستان نسائي", ImageUrl = "/images/yemeni_balto.png", IsActive = true, CreatedAt = DateTime.UtcNow },
                new Product { Name = "بالطو مطرز بالزري", Description = "بوالاط مناسبات وأعراس بتطريز زري فاخر", Price = 65.00m, StockQuantity = 25, Category = "فستان نسائي", ImageUrl = "/images/yemeni_balto_embroidered.png", IsActive = true, CreatedAt = DateTime.UtcNow },
                new Product { Name = "جلابية يمنية مطرزة", Description = "جلابيات واستقبال وتصميم تراثي مميز", Price = 55.00m, StockQuantity = 40, Category = "فستان نسائي", ImageUrl = "/images/yemeni_jalabiya.png", IsActive = true, CreatedAt = DateTime.UtcNow },
                new Product { Name = "زَنّة يمنية مطرزة", Description = "خياطة زنين يمنية ومناسبات تقليدية", Price = 60.00m, StockQuantity = 35, Category = "فستان نسائي", ImageUrl = "/images/yemeni_zenneh.png", IsActive = true, CreatedAt = DateTime.UtcNow },
                new Product { Name = "فستان درّاعة يمنية", Description = "فساتين وسهرات راقية بتصاميم حديثة", Price = 75.00m, StockQuantity = 20, Category = "فستان نسائي", ImageUrl = "/images/yemeni_dress.png", IsActive = true, CreatedAt = DateTime.UtcNow },
                new Product { Name = "ثوب صنعاني مطرز", Description = "أزياء شعبية وتراثية صنعانية فاخرة", Price = 90.00m, StockQuantity = 15, Category = "فستان نسائي", ImageUrl = "/images/yemeni_traditional.png", IsActive = true, CreatedAt = DateTime.UtcNow }
            );
            context.SaveChanges();
        }

        if (!context.Orders.Any())
        {
            context.Orders.AddRange(
                new Order
                {
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
                    CreatedAt = DateTime.UtcNow
                },
                new Order
                {
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
                    CreatedAt = DateTime.UtcNow
                },
                new Order
                {
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
                    CreatedAt = DateTime.UtcNow
                },
                new Order
                {
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
                    CreatedAt = DateTime.UtcNow
                },
                new Order
                {
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
                    CreatedAt = DateTime.UtcNow
                },
                new Order
                {
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
                    CreatedAt = DateTime.UtcNow
                }
            );
            context.SaveChanges();
        }
    }
    catch { }
}

// تفعيل Swagger/Scalar كواجهة واختبار الـ API
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
    app.MapGet("/", () => Results.Redirect("/login/"));

    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Maqas API v1");
        c.RoutePrefix = "swagger";
    });
}

// إعادة توجيه الروابط القديمة التي تنتهي بـ .html في الجذر الرئيسي تلقائياً إلى المجلد المنظم
app.Use(async (context, next) =>
{
    var path = context.Request.Path.Value;
    if (!string.IsNullOrEmpty(path) && path.EndsWith(".html", StringComparison.OrdinalIgnoreCase))
    {
        var fileName = System.IO.Path.GetFileNameWithoutExtension(path);
        if (!path.TrimStart('/').Contains("/"))
        {
            context.Response.Redirect("/" + fileName + "/");
            return;
        }
    }
    await next();
});

app.UseCors("AllowAll");
app.UseDefaultFiles();
app.UseStaticFiles();

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
