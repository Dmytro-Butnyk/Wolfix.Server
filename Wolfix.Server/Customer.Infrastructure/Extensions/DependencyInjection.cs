using Customer.Domain.Interfaces;
using Customer.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Shared.Domain.Interfaces;
using Shared.Infrastructure.Repositories;

namespace Customer.Infrastructure.Extensions;

public static class DependencyInjection
{
    public static IServiceCollection AddCustomerDbContext(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<CustomerContext>(options =>
            options.UseNpgsql(connectionString));
        
        return services;
    }

    public static IServiceCollection AddCustomerRepositories(this IServiceCollection services)
    {
        services.AddScoped(typeof(IBaseRepository<Customer.Domain.CustomerAggregate.Customer>),
            typeof(BaseRepository<CustomerContext, Customer.Domain.CustomerAggregate.Customer>));

        services.AddScoped<ICustomerRepository, CustomerRepository>();
        
        return services;
    }
}