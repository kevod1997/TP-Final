using ProductService.API;
using ProductService.Infrastructure;
using System.Text.Json.Serialization;
using TrabajoFinal.Common.Shared.Logging;
using ProductService.Application;

var builder = WebApplication.CreateBuilder(args);

// Configurar Serilog usando nuestra extensión compartida
builder.Host.ConfigureStandardLogging("ProductService");

// Agregar servicios de logging al contenedor DI
builder.Services.AddStandardLogging();

// Agregar controladores con opciones de JSON
builder.Services.AddControllers()
    .AddJsonOptions(opts =>
    {
        opts.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

// Configurar Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Product Service API", Version = "v1" });
});

// Registrar servicios de aplicación
builder.Services.AddApplication();

// Registrar servicios de infraestructura
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

// Configurar el pipeline de solicitudes HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseExceptionHandling();
app.UseRequestResponseLogging();

app.UseAuthorization();

app.MapControllers();

app.Run();