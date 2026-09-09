var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

app.UseStaticFiles();

app.MapGet("/", () => Results.Redirect("/login/login.html"));

app.Run();
