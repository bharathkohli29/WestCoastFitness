using Microsoft.Extensions.DependencyInjection;
using WestCoastFitness.Application.Access;
using WestCoastFitness.Application.Auth;
using WestCoastFitness.Application.Memberships;
using WestCoastFitness.Application.Scheduling;
using WestCoastFitness.Application.Payments;

namespace WestCoastFitness.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<MembershipPricingService>();
        services.AddScoped<MembershipCommandService>();
        services.AddScoped<ClassCapacityService>();
        services.AddScoped<ClassSchedulingCommandService>();
        services.AddScoped<TrainerAvailabilityService>();
        services.AddScoped<MemberAccessService>();
        services.AddScoped<PaymentActivationService>();
        services.AddScoped<AuthService>();

        return services;
    }
}
