using CustomerService.API.Middleware;
using CustomerService.Application;
using CustomerService.Infrastructure;
using TrabajoFinal.Common.Shared.Logging;
using TrabajoFinal.Common.Shared.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Configurar logging
builder.Host.ConfigureStandardLogging("CustomerService");
builder.Services.AddStandardLogging();

// Configurar servicios
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Registrar capas
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

// Configuración CORS
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

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Habilitar CORS
app.UseCors("AllowAll");

// Añadir middleware personalizado
app.UseExceptionHandling();
app.UseRequestResponseLogging();
app.UseResultTransformation();

app.UseAuthorization();

app.MapControllers();

app.Run();