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
