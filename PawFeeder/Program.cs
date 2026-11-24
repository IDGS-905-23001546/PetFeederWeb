using Microsoft.EntityFrameworkCore;
using PawFeeder.Data;
using PawFeeder.Models;
using PawFeeder.Services;

var builder = WebApplication.CreateBuilder(args);

var connectionString =
    builder.Configuration.GetConnectionString("PetFeederConnection");


// ================================
// CONEXI�N SQL SERVER + EF CORE
// ================================
builder.Services.AddDbContext<PawFeederContext>(options =>
{
    options.UseSqlServer(connectionString)
           .UseSnakeCaseNamingConvention();
});


// ================================
// CORREO ELECTR�NICO
// ================================
builder.Services.Configure<EmailSettings>(
    builder.Configuration.GetSection("EmailSettings"));

builder.Services.AddSingleton<EmailService>();


// MVC + API
builder.Services.AddControllersWithViews();


// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


// ================================
// CORS ANGULAR / FRONTEND
// ================================
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", policy =>
    {
        policy.WithOrigins(
                "http://localhost:4200",
                "http://localhost:4000",
                "http://localhost:5173",
                "https://pawfeeder-web.onrender.com")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});


var app = builder.Build();


// CORS
app.UseCors("AllowAngular");


// Swagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();

    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint(
            "/swagger/v1/swagger.json",
            "PawFeeder API v1"
        );
    });
}


// Nota: la redirección HTTPS se desactiva porque rompe el CORS del frontend
// local (localhost:4200) al redirigir 5169 -> 7122 con certificado no confiable.
// En producción se recomienda servir solo HTTPS a nivel del host/proxy.
// app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();


// API
app.MapControllers();


// MVC
app.MapControllerRoute(
    name: "app",
    pattern: "app/{action=Index}/{id?}",
    defaults: new { controller = "App" }
);


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
);


app.Run();