using Application.Common.Interfaces.Authorization;
using Application.Common.Interfaces.Entities;
using Application.Common.Interfaces.Entities.AdoptionAlertNotifications;
using Application.Common.Interfaces.Entities.AdoptionFavoriteAlerts;
using Application.Common.Interfaces.Entities.AdoptionReports;
using Application.Common.Interfaces.Entities.Alerts.AdoptionAlerts;
using Application.Common.Interfaces.Entities.Alerts.FoundAnimalAlerts;
using Application.Common.Interfaces.Entities.Alerts.MissingAlerts;
using Application.Common.Interfaces.Entities.Alerts.UserPreferences;
using Application.Common.Interfaces.Entities.AnimalSpecies;
using Application.Common.Interfaces.Entities.Breeds;
using Application.Common.Interfaces.Entities.Colors;
using Application.Common.Interfaces.Entities.Pets;
using Application.Common.Interfaces.Entities.UserMessages;
using Application.Common.Interfaces.Entities.Users;
using Application.Common.Interfaces.ExternalServices.AWS;
using Application.Common.Interfaces.ExternalServices.GeoLocation;
using Application.Common.Interfaces.Localization;
using Application.Common.Interfaces.Messaging;
using Application.Common.Interfaces.Persistence;
using Application.Common.Interfaces.RealTimeCommunication;
using Application.Common.Interfaces.SharingAlerts;
using Infrastructure.DependencyInjections;
using Infrastructure.ExternalServices.Auth;
using Infrastructure.ExternalServices.AWS;
using Infrastructure.ExternalServices.Configs;
using Infrastructure.ExternalServices.GeoLocation;
using Infrastructure.Messaging;
using Infrastructure.Persistence.DataAccessObjects;
using Infrastructure.Persistence.DataContext;
using Infrastructure.Persistence.Interceptors;
using Infrastructure.Persistence.Repositories;
using Infrastructure.RealTimeCommunication;
using Infrastructure.SharingAlerts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IPetRepository, PetRepository>();
        services.AddScoped<IBreedRepository, BreedRepository>();
        services.AddScoped<IColorRepository, ColorRepository>();
        services.AddScoped<ISpeciesRepository, SpeciesRepository>();
        services.AddScoped<IMissingAlertRepository, MissingAlertRepository>();
        services.AddScoped<ISpeciesRepository, SpeciesRepository>();
        services.AddScoped<IAdoptionAlertRepository, AdoptionAlertRepository>();
        services.AddScoped<IUserMessageRepository, UserMessageRepository>();
        services.AddScoped<IMessagingService, MessagingService>();
        services.AddScoped<IFoundAnimalAlertRepository, FoundAnimalAlertRepository>();
        services.AddScoped<IFoundAnimalUserPreferencesRepository, FoundAnimalUserPreferencesRepository>();
        services.AddScoped<IAdoptionUserPreferencesRepository, AdoptionUserPreferencesRepository>();
        services.AddScoped<IExternalAuthHandler, ExternalAuthHandler>();
        services.AddScoped<IAdoptionFavoritesRepository, AdoptionFavoritesRepository>();
        services.AddScoped<IRealTimeChatClient, RealTimeChatClient>();
        services.AddScoped<IGeoLocationClient, GeoLocationClient>();
        services.AddScoped<ILocalizationRepository, LocalizationRepository>();
        services.AddScoped<IPetImageRepository, PetImageRepository>();
        services.AddScoped<IAdoptionReportRepository, AdoptionReportRepository>();
        services.AddScoped<IAdoptionAlertNotificationsRepository, AdoptionAlertNotificationsRepository>();
        services.AddScoped<IUserDao, UserDao>();

        services.AddScoped<IFileUploadClient, FileUploadClient>();
        services.Configure<AwsData>(configuration.GetSection("AWS"));
        services.ConfigureAws(configuration);

        services.AddScoped<ISharingPosterGenerator, SharingPosterGenerator>();

        services.AddScoped<PublishDomainEventsInterceptor>();
        services.AddScoped<IAppDbContext, AppDbContext>();
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection") ?? string.Empty,
                    opt => opt.UseNetTopologySuite())
                .UseEnumCheckConstraints());

        services.Configure<GeoLocationApiConfig>(configuration.GetSection("GeoLocationApi"));
        services.AddHttpClient(GeoLocationApiConfig.ClientKey,
            (serviceProvider, client) =>
            {
                var config = serviceProvider.GetRequiredService<IOptions<GeoLocationApiConfig>>().Value;
                client.BaseAddress = new Uri(config.BaseUrl);
                client.DefaultRequestHeaders.UserAgent.ParseAdd("AcheMeuPet");
            });

        services.AddHttpClient(ViaCepApiConfig.ClientKey,
            client => { client.BaseAddress = ViaCepApiConfig.BaseUrl; });

        services.AddHttpClient(FacebookGraphApiConfig.ClientKey,
            client => { client.BaseAddress = FacebookGraphApiConfig.BaseUrl; });

        services.AddHttpClient(ImageStorageApiConfig.ClientKey,
            (serviceProvider, client) =>
            {
                var config = serviceProvider.GetRequiredService<IOptions<AwsData>>().Value;
                client.BaseAddress = new Uri(config.ServiceDomain);
            });

        return services;
    }
}