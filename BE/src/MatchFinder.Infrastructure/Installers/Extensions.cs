using MatchFinder.Infrastructure.Hubs;
using MatchFinder.Infrastructure.Services;
using MatchFinder.Infrastructure.Services.Core;
using MatchFinder.Infrastructure.Services.Impl;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Net.payOS;

namespace MatchFinder.Infrastructure.Installers
{
    public static class Extensions
    {
        public static void AddInfrastructureServices(this IServiceCollection services)
        {
            services.AddHttpContextAccessor();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IFileService, FileService>();
            services.AddScoped<IPayOSPaymentService>(sp =>
            {
                var payOSSettings = sp.GetRequiredService<IOptions<PayOSSettings>>();
                var payOS = new PayOS(
                  payOSSettings.Value.ClientId,
                payOSSettings.Value.ApiKey,
                payOSSettings.Value.ChecksumKey
            );
                return new PayOSPaymentService(payOS, payOSSettings);
            });
            services.AddSignalR();
        }

        public static void UseInfrastructureEndpointRoute(this IEndpointRouteBuilder app)
        {
            app.MapHub<NotificationHub>("/hub/notifications");
        }
    }
}