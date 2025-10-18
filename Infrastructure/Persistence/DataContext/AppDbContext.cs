using System.Reflection;
using Application.Common.Interfaces.Persistence;
using Domain.Common;
using Domain.Entities;
using Domain.Entities.Alerts;
using Domain.Entities.Alerts.Notifications;
using Domain.Entities.Alerts.UserFavorites;
using Domain.Entities.Alerts.UserPreferences;
using Domain.ValueObjects;
using Infrastructure.Persistence.DataSeed;
using Infrastructure.Persistence.Interceptors;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.DataContext;

public class AppDbContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>, IAppDbContext
{
    private readonly PublishDomainEventsInterceptor _publishDomainEventsInterceptor;

    public AppDbContext(DbContextOptions<AppDbContext> options,
        PublishDomainEventsInterceptor publishDomainEventsInterceptor) : base(options)
    {
        _publishDomainEventsInterceptor = publishDomainEventsInterceptor;
    }

    public DbSet<Pet> Pets { get; set; } = null!;
    public DbSet<Breed> Breeds { get; set; } = null!;
    public DbSet<Color> Colors { get; set; } = null!;
    public DbSet<Species> Species { get; set; } = null!;
    public DbSet<MissingAlert> MissingAlerts { get; set; } = null!;
    public DbSet<AdoptionAlert> AdoptionAlerts { get; set; } = null!;
    public DbSet<FoundAnimalAlert> FoundAnimalAlerts { get; set; } = null!;
    public DbSet<UserMessage> UserMessages { get; set; } = null!;
    public DbSet<PetImage> PetImage { get; set; } = null!;
    public DbSet<FoundAnimalAlertImage> FoundAnimalAlertImages { get; set; } = null!;
    public DbSet<FoundAnimalUserPreferences> FoundAnimalUserPreferences { get; set; } = null!;
    public DbSet<AdoptionUserPreferences> AdoptionUserPreferences { get; set; } = null!;
    public DbSet<AdoptionFavorite> AdoptionFavorites { get; set; } = null!;
    public DbSet<City> Cities { get; set; } = null!;
    public DbSet<State> States { get; set; } = null!;
    public DbSet<AdoptionReport> AdoptionReports { get; set; } = null!;
    public DbSet<AdoptionAlertNotification> AdoptionAlertNotifications { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder
            .Ignore<List<IDomainEvent>>()
            .ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(builder);

        builder.HasPostgresExtension("unaccent");
        builder.HasPostgresExtension("postgis");

        builder.Entity<Breed>().HasData(SeedBreeds.Seed());
        builder.Entity<Color>().HasData(SeedColors.Seed());
        builder.Entity<Species>().HasData(SeedSpecies.Seed());
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.AddInterceptors(_publishDomainEventsInterceptor);

        base.OnConfiguring(optionsBuilder);
    }
}