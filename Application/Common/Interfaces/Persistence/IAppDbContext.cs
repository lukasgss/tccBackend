using Domain.Entities;
using Domain.Entities.Alerts;
using Domain.Entities.Alerts.Notifications;
using Domain.Entities.Alerts.UserFavorites;
using Domain.Entities.Alerts.UserPreferences;
using Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace Application.Common.Interfaces.Persistence;

public interface IAppDbContext
{
    DbSet<User> Users { get; set; }
    public DbSet<Pet> Pets { get; set; }
    public DbSet<Breed> Breeds { get; set; }
    public DbSet<Color> Colors { get; set; }
    public DbSet<Species> Species { get; set; }
    public DbSet<MissingAlert> MissingAlerts { get; set; }
    public DbSet<AdoptionAlert> AdoptionAlerts { get; set; }
    public DbSet<FoundAnimalAlert> FoundAnimalAlerts { get; set; }
    public DbSet<UserMessage> UserMessages { get; set; }
    public DbSet<PetImage> PetImage { get; set; }
    public DbSet<FoundAnimalAlertImage> FoundAnimalAlertImages { get; set; }
    public DbSet<FoundAnimalUserPreferences> FoundAnimalUserPreferences { get; set; }
    public DbSet<AdoptionUserPreferences> AdoptionUserPreferences { get; set; }
    public DbSet<AdoptionFavorite> AdoptionFavorites { get; set; }
    public DbSet<FoundAnimalFavorite> FoundAnimalFavorites { get; set; }
    public DbSet<City> Cities { get; set; }
    public DbSet<State> States { get; set; }
    public DbSet<AdoptionAlertNotification> AdoptionAlertNotifications { get; set; }
}