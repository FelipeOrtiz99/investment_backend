using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using InvestmentBackend.Application.Transactions.Services;

namespace InvestmentBackend.Application;

public static class ApplicationServicesRegistration
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Register MediatR
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

        // Register Application Services
        services.AddScoped<ITransactionApplicationService, TransactionApplicationService>();

        return services;
    }
}
