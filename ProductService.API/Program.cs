using ProductService.API;
using ProductService.Infrastructure;
using System.Text.Json.Serialization;
using TrabajoFinal.Common.Shared.Logging;
using ProductService.Application;
using TrabajoFinal.Common.Shared.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Configurar Serilog
builder.Host.ConfigureStandardLogging("ProductService");

// Agregar servicios al contenedor
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Agregar capas de la aplicaci�n
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

// Agregar servicios comunes
builder.Services.AddStandardLogging();

var app = builder.Build();

// Configurar el pipeline HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Agregar middleware personalizado
app.UseExceptionHandling();
app.UseRequestResponseLogging();
app.UseResultTransformation(); // Nuevo middleware para transformar Result

app.UseAuthorization();

app.MapControllers();

app.Run();