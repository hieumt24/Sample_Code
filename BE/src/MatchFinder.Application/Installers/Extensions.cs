using AutoMapper;
using MatchFinder.Application.Authorize.Interfaces;
using MatchFinder.Application.Authorize.Services;
using MatchFinder.Application.Configurations;
using MatchFinder.Application.Services;
using MatchFinder.Application.Services.Impl;
using MatchFinder.Domain.Interfaces;
using MatchFinder.Infrastructure.Helpers;
using MatchFinder.Infrastructure.UnitOfWork;
using Microsoft.Extensions.DependencyInjection;

namespace MatchFinder.Application.Installers
{
    public static class Extensions
    {
        public static void AddApplicationServices(this IServiceCollection services)
        {
            var mapperConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new AutoMapperProfile());
            });

            IMapper mapper = mapperConfig.CreateMapper();
            services.AddSingleton(mapper);
            services.AddScoped<ICryptographyHelper, CryptographyHelper>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IVerificationService, VerificationService>();
            services.AddScoped<IPartialFieldService, PartialFieldService>();
            services.AddScoped<IFieldService, FieldService>();
            services.AddScoped<IInactiveTimeService, InactiveTimeService>();
            services.AddScoped<IBookingService, BookingService>();
            services.AddScoped<ITeamService, TeamService>();
            services.AddScoped<IMenuService, MenuService>();
            services.AddScoped<ITransactionService, TransactionService>();
            services.AddScoped<IOpponentFindingService, OpponentFindingService>();
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<IRateService, RateService>();
            services.AddScoped<ISlotService, SlotService>();
            services.AddScoped<IFavoriteFieldService, FavoriteFieldService>();
            services.AddScoped<IStatisticService, StatisticService>();
            services.AddScoped<IReportService, ReportService>();
            services.AddScoped<IImageService, ImageService>();
            services.AddScoped<IBlogPostService, BlogPostService>();
            services.AddScoped<IStaffService, StaffService>();

            // Register author
            services.AddScoped<IUserAuthenticator, UserAuthenticator>();
            services.AddScoped<IRequestIdExtractor, RequestIdExtractor>();
            services.AddScoped<IBookingAuthorizer, BookingAuthorizer>();
            services.AddScoped<IFieldOwnershipAuthorizer, FieldOwnershipAuthorizer>();
            services.AddScoped<IFieldAuthorizer, FieldAuthorizer>();
            services.AddScoped<IPartialFieldAuthorizer, PartialFieldAuthorizer>();
            services.AddScoped<IReportAuthorizer, ReportAuthorizer>();
            services.AddScoped<IInactiveTimeAuthorizer, InactiveTimeAuthorizer>();
        }
    }
}