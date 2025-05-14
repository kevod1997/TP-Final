using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http;
using OrderService.Application.Services;
using OrderService.Domain.Repositories;
using OrderService.Infrastructure.Data;
using OrderService.Infrastructure.Repositories;
using OrderService.Infrastructure.Services;


namespace OrderService.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            // Registrar DbContext
            services.AddDbContext<OrderDbContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection"),
                    b => b.MigrationsAssembly(typeof(OrderDbContext).Assembly.FullName)));

            // Registrar repositorios
            services.AddScoped<IOrderRepository, OrderRepository>();

            // Registrar servicios HTTP para comunicación con otros microservicios
            // Sin políticas de resiliencia por ahora para simplificar
            services.AddHttpClient<ICustomerService, CustomerService>();
            services.AddHttpClient<IProductService, ProductService>();

            return services;
        }
    }
}