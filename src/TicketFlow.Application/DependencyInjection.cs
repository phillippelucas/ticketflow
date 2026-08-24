using Microsoft.Extensions.DependencyInjection;
using TicketFlow.Application.Eventos;

namespace TicketFlow.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddSingleton(TimeProvider.System);
        services.AddScoped<IEventoService, EventoService>();

        return services;
    }
}
