using System.Text.Json.Serialization;
using Maqas.Infrastructure;
using Maqas.Application.Common.Interfaces;
using Maqas.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

// 1. إضافة خدمات MVC والـ Controllers
builder.Services.AddControllersWithViews()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });

// 2. إضافة طبقة البنية التحتية والبيانات
builder.Services.AddInfrastructure(builder.Configuration);

// 3. إضافة MediatR من طبقة Application
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(IAppDbContext).Assembly));

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

var app = builder.Build();

// تهيئة قاعدة البيانات والتأكد من إعداد البيانات الأولية
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    context.Database.EnsureCreated();
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseCors("AllowAll");
app.UseDefaultFiles();
app.UseStaticFiles();

// إعادة توجيه الروابط القديمة التي تنتهي بـ .html إلى المسارات المنظمة
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

app.UseRouting();
app.UseAuthorization();

app.MapControllers();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
