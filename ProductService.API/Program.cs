using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ProductService.API;
using ProductService.Application.Behaviors;
using ProductService.Application.Commands;
using ProductService.Application.Mappings;
using ProductService.Domain.Repositories;
using ProductService.Infrastructure.Data;
using ProductService.Infrastructure.Repositories;
using Serilog;


var builder = WebApplication.CreateBuilder(args);

// Configurar Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("logs/product-service-.log", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

// Agregar servicios al contenedor
builder.Services.AddControllers();

// Configurar FluentValidation
builder.Services.AddValidatorsFromAssemblyContaining<CreateProductCommandValidator>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Agregar DbContext
builder.Services.AddDbContext<ProductDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        b => b.MigrationsAssembly(typeof(ProductDbContext).Assembly.FullName)));

// Registrar repositorios
builder.Services.AddScoped<IProductRepository, ProductRepository>();

// Registrar AutoMapper
builder.Services.AddAutoMapper(typeof(MappingProfile));

// Registrar MediatR con el ValidationBehavior
builder.Services.AddMediatR(cfg => {
    cfg.RegisterServicesFromAssembly(typeof(CreateProductCommand).Assembly);
    cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
});

var app = builder.Build();

app.UseExceptionHandling();
app.UseRequestResponseLogging();

// Configurar middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();

// Ya no necesitas el UseExceptionHandler si tienes tu propio middleware de excepciones
// app.UseExceptionHandler("/error");

app.MapControllers();

// Aplicar migraciones automáticamente en desarrollo
if (app.Environment.IsDevelopment())
{
    using (var scope = app.Services.CreateScope())
    {
        var context = scope.ServiceProvider.GetRequiredService<ProductDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        try
        {
            context.Database.Migrate();
            logger.LogInformation("Database migrated successfully.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while migrating the database.");
        }
    }
}

app.Run();