using Microsoft.Extensions.DependencyInjection;
using Support.Application.Services;

namespace Support.Application.Extensions;

public static class DependencyInjection
{
    public static IServiceCollection AddSupportApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<SupportRequestService>();

        return services;
    }
}