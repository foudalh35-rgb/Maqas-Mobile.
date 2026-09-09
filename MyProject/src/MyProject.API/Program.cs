using MyProject.API.Extensions;
using MyProject.API.Middleware;
using MyProject.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add API services (Controllers, Swagger, CORS)
builder.Services.AddApiServices();

// Add Infrastructure services (DbContext, Repositories)
builder.Services.AddInfrastructure(builder.Configuration);

// Add MediatR
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(MyProject.Application.Features.Products.Commands.CreateProduct.CreateProductCommand).Assembly));

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();

app.Run();
