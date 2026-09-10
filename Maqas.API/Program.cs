using Scalar.AspNetCore;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Maqas.Infrastructure;
using Maqas.Application.Common.Interfaces;
using Maqas.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

// 1. إضافة طبقة البنية التحتية (Infrastructure)
builder.Services.AddInfrastructure(builder.Configuration);

// 2. إضافة MediatR من طبقة التطبيق (Application)
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(IAppDbContext).Assembly));

// 3. إضافة المتحكمات (Controllers) وتكوين خيارات JSON
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });

// 4. سياسة CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// 5. توثيق OpenAPI / Swagger / Scalar
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// تهيئة قاعدة البيانات والتأكد من إعداد البيانات الأولية
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    context.Database.EnsureCreated();
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Maqas Clean Architecture API v1");
        c.RoutePrefix = "swagger";
    });
}

// إعادة توجيه الروابط القديمة التي تنتهي بـ .html تلقائياً إلى المجلد المنظم
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

// التوجيه إلى واجهة تسجيل الدخول الرئيسية للواجهات بدلاً من Swagger تلقائياً عند فتح الجذر
app.MapGet("/", () => Results.Redirect("/login/login.html"));

app.Run();
