using System.Reflection;
using Application.Common.Behaviors;
using Application.Common.Interfaces.Authentication;
using Application.Common.Interfaces.Authorization;
using Application.Common.Interfaces.Converters;
using Application.Common.Interfaces.Entities.Alerts.FoundAnimalAlerts;
using Application.Common.Interfaces.General.Files;
using Application.Common.Interfaces.General.UserPreferences;
using Application.Common.Interfaces.Messaging;
using Application.Common.Interfaces.Providers;
using Application.Common.Providers;
using Application.Marker;
using Application.Services.Authentication;
using Application.Services.Authorization;
using Application.Services.Converters;
using Application.Services.Entities;
using Application.Services.General.Files;
using Application.Services.General.Messages;
using Application.Services.General.UserPreferences;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using FluentValidation;
using MediatR;

namespace Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));
        });

        services.AddScoped<IUserAuthorizationService, UserAuthorizationService>();
        services.AddScoped<IIdConverterService, IdConverterService>();
        services.AddScoped<IImageProcessingService, ImageProcessingService>();
        services.AddScoped<IFoundAnimalAlertService, FoundAnimalAlertService>();
        services.AddScoped<IPetImageSubmissionService, PetImageSubmissionService>();
        services.AddScoped<IFoundAlertImageSubmissionService, FoundAlertImageSubmissionService>();
        services.AddScoped<IUserPreferencesValidations, UserPreferencesValidations>();
        services.AddScoped<IAdoptionAlertFileSubmissionService, AdoptionAlertFileSubmissionService>();

        services.AddValidatorsFromAssemblyContaining<IAssemblyMarker>();

        services.AddSingleton<IValueProvider, ValueProvider>();
        services.AddSingleton<ITokenGenerator, TokenGenerator>();

        services.Configure<MessagingSettings>(configuration.GetSection("MessagingSettings"));
        services.Configure<ImagesData>(configuration.GetSection("ImagesData"));

        return services;
    }
}