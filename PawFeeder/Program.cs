using Microsoft.EntityFrameworkCore;
using PawFeeder.Data;
using PawFeeder.Models;
using PawFeeder.Services;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("PetFeederConnection");

// 2. Pasamos la variable al DbContext
builder.Services.AddDbContext<PawFeederContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddControllersWithViews();

builder.Services.Configure<EmailSettings>(
    builder.Configuration.GetSection("EmailSettings"));

builder.Services.AddScoped<EmailService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", policy =>
    {
        policy.WithOrigins("http://localhost:4200") // El puerto de tu Angular
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// 1. Ponemos el CORS al principio del pipeline para interceptar todo antes del enrutamiento
app.UseCors("AllowAngular");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "PawFeeder API v1");
    });
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

// 2. Después de CORS y estáticos va el enrutamiento y la autorización
app.UseRouting();
app.UseAuthorization();

// 3. Mapeo de controladores para tu API
app.MapControllers(); // <-- Añade esta línea si tus controladores usan [ApiController]

// Rutas de MVC que ya tenías
app.MapControllerRoute(
    name: "app",
    pattern: "app/{action=Index}/{id?}",
    defaults: new { controller = "App" });

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
