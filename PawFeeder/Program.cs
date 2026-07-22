using Microsoft.EntityFrameworkCore;
using PawFeeder.Data;
using PawFeeder.Services;

var builder = WebApplication.CreateBuilder(args);

var connectionString =
    builder.Configuration.GetConnectionString("PetFeederConnection");


// ================================
// CONEXIÓN SQL SERVER + EF CORE
// ================================
builder.Services.AddDbContext<PawFeederContext>(options =>
{
    options.UseSqlServer(connectionString)
           .UseSnakeCaseNamingConvention();
});


// MVC + API
builder.Services.AddControllersWithViews();


// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


// ================================
// CORS ANGULAR
// ================================
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
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


app.UseHttpsRedirection();

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