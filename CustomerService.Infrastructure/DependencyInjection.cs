using CustomerService.Domain.Repositories;
using CustomerService.Infrastructure.Data;
using CustomerService.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CustomerService.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            // Registrar DbContext
            services.AddDbContext<CustomerDbContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection"),
                    b => b.MigrationsAssembly(typeof(CustomerDbContext).Assembly.FullName)));

            // Registrar repositorios
            services.AddScoped<ICustomerRepository, CustomerRepository>();

            return services;
        }
    }
}