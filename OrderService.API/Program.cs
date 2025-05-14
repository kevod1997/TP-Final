using Microsoft.EntityFrameworkCore;
using OrderService.API.Middleware;
using OrderService.Application;
using OrderService.Infrastructure;
using OrderService.Infrastructure.Data;
using TrabajoFinal.Common.Shared.Logging;
using TrabajoFinal.Common.Shared.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Configurar Serilog
builder.Host.ConfigureStandardLogging("OrderService");

// A�adir servicios al contenedor
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Registrar servicios de aplicaci�n e infraestructura
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

// Registrar servicios comunes
builder.Services.AddStandardLogging();

// Configuraci�n CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configurar la canalizaci�n de HTTP request
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    // Crear/actualizar la base de datos en desarrollo
    using (var scope = app.Services.CreateScope())
    {
        var db = scope.ServiceProvider.GetRequiredService<OrderDbContext>();
        db.Database.Migrate();
    }
}

app.UseHttpsRedirection();

app.UseCors("AllowAll");

// Middleware personalizado
app.UseExceptionHandling();
app.UseRequestResponseLogging();
app.UseResultTransformation();

app.UseAuthorization();

app.MapControllers();

app.Run();
