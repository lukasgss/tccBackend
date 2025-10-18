using Application.Common.Interfaces.Entities.Alerts.UserPreferences;
using Application.Events.AdoptionAlerts.CreatedAdoptionAlert;
using Domain.Entities.Alerts.UserPreferences;
using Domain.Events;
using Infrastructure.Persistence.DataContext;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using NpgsqlTypes;

namespace Infrastructure.Persistence.Repositories;

public class AdoptionUserPreferencesRepository : GenericRepository<AdoptionUserPreferences>,
    IAdoptionUserPreferencesRepository
{
    private readonly AppDbContext _dbContext;

    public AdoptionUserPreferencesRepository(AppDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public async Task<AdoptionUserPreferences?> GetUserPreferences(Guid userId)
    {
        return await _dbContext.AdoptionUserPreferences
            .Include(preferences => preferences.Breeds)
            .Include(preferences => preferences.User)
            .Include(preferences => preferences.Colors)
            .Include(preferences => preferences.Species)
            .SingleOrDefaultAsync(preferences => preferences.User.Id == userId);
    }

    public async Task<List<UserThatMatchPreferences>> GetUsersThatMatchPreferences(AdoptionAlertCreated adoptionAlert,
        double matchThreshold = 0.6)
    {
        // Breeds and species are a must to be matched, otherwise, it uses a match thresold of
        // 60%, so if the pet characteristics match 60%, it notifies the user.
        string sql = @"
            WITH ""PreferenceMatches"" AS (
                SELECT 
                    ""p"".""UserId"",
                    (
                        CASE WHEN EXISTS(
                            SELECT 1 FROM ""AdoptionUserPreferencesBreed"" b 
                            WHERE b.""AdoptionUserPreferencesId"" = ""p"".""Id"" 
                            AND b.""BreedsId"" = @breedId
                        ) THEN 1 ELSE 0 END +
                        
                        CASE WHEN EXISTS(
                            SELECT 1 FROM ""AdoptionUserPreferencesColor"" c 
                            WHERE c.""AdoptionUserPreferencesId"" = ""p"".""Id"" 
                            AND c.""ColorsId"" = ANY(@colorIds::integer[])
                        ) THEN 1 ELSE 0 END +
                        
                        CASE WHEN EXISTS(
                            SELECT 1 FROM ""AdoptionUserPreferencesSpecies"" s 
                            WHERE s.""AdoptionUserPreferencesId"" = ""p"".""Id"" 
                            AND s.""SpeciesId"" = @speciesId
                        ) THEN 1 ELSE 0 END +
                        
                        CASE WHEN @gender = ANY(""p"".""Genders"") THEN 1 ELSE 0 END +
                        CASE WHEN @age = ANY(""p"".""Ages"") THEN 1 ELSE 0 END +
                        CASE WHEN @size = ANY(""p"".""Sizes"") THEN 1 ELSE 0 END
                    ) AS ""MatchCount"",
                    (
                        CASE WHEN EXISTS(
                            SELECT 1 FROM ""AdoptionUserPreferencesBreed"" b 
                            WHERE b.""AdoptionUserPreferencesId"" = ""p"".""Id""
                        ) THEN 1 ELSE 0 END +
                        
                        CASE WHEN EXISTS(
                            SELECT 1 FROM ""AdoptionUserPreferencesColor"" c 
                            WHERE c.""AdoptionUserPreferencesId"" = ""p"".""Id""
                        ) THEN 1 ELSE 0 END +
                        
                        CASE WHEN EXISTS(
                            SELECT 1 FROM ""AdoptionUserPreferencesSpecies"" s 
                            WHERE s.""AdoptionUserPreferencesId"" = ""p"".""Id""
                        ) THEN 1 ELSE 0 END +
                        
                        CASE WHEN array_length(""p"".""Genders"", 1) > 0 THEN 1 ELSE 0 END +
                        CASE WHEN array_length(""p"".""Ages"", 1) > 0 THEN 1 ELSE 0 END +
                        CASE WHEN array_length(""p"".""Sizes"", 1) > 0 THEN 1 ELSE 0 END
                    ) AS ""TotalPreferencesSet"",
                    CASE WHEN EXISTS(
                        SELECT 1 FROM ""AdoptionUserPreferencesBreed"" b 
                        WHERE b.""AdoptionUserPreferencesId"" = ""p"".""Id""
                    ) THEN 
                        CASE WHEN EXISTS(
                            SELECT 1 FROM ""AdoptionUserPreferencesBreed"" b 
                            WHERE b.""AdoptionUserPreferencesId"" = ""p"".""Id"" 
                            AND b.""BreedsId"" = @breedId
                        ) THEN true ELSE false END
                    ELSE true END AS ""BreedMatches"",
                    CASE WHEN EXISTS(
                        SELECT 1 FROM ""AdoptionUserPreferencesSpecies"" s 
                        WHERE s.""AdoptionUserPreferencesId"" = ""p"".""Id""
                    ) THEN 
                        CASE WHEN EXISTS(
                            SELECT 1 FROM ""AdoptionUserPreferencesSpecies"" s 
                            WHERE s.""AdoptionUserPreferencesId"" = ""p"".""Id"" 
                            AND s.""SpeciesId"" = @speciesId
                        ) THEN true ELSE false END
                    ELSE true END AS ""SpeciesMatches""
                FROM ""AdoptionUserPreferences"" ""p""
            )
            SELECT DISTINCT ""UserId""
            FROM ""PreferenceMatches""
            WHERE ""TotalPreferencesSet"" > 0 
            AND ""BreedMatches"" = true
            AND ""SpeciesMatches"" = true
            AND ""UserId"" != @userId
            AND CAST(""MatchCount"" AS FLOAT) / CAST(""TotalPreferencesSet"" AS FLOAT) >= @matchThreshold";

        var parameters = new object[]
        {
            new NpgsqlParameter("breedId", NpgsqlDbType.Integer) { Value = adoptionAlert.BreedId },
            new NpgsqlParameter<int[]>("colorIds", adoptionAlert.ColorIds.ToArray()),
            new NpgsqlParameter("speciesId", NpgsqlDbType.Integer) { Value = adoptionAlert.SpeciesId },
            new NpgsqlParameter("gender", NpgsqlDbType.Integer) { Value = (int)adoptionAlert.Gender },
            new NpgsqlParameter("age", NpgsqlDbType.Integer) { Value = (int)adoptionAlert.Age },
            new NpgsqlParameter("size", NpgsqlDbType.Integer) { Value = (int)adoptionAlert.Size },
            new NpgsqlParameter("matchThreshold", NpgsqlDbType.Double) { Value = matchThreshold },
            new NpgsqlParameter("userId", NpgsqlDbType.Uuid) { Value = adoptionAlert.OwnerId }
        };

        var userIds = await _dbContext.Database
            .SqlQueryRaw<Guid>(sql, parameters)
            .ToListAsync();

        return userIds.Select(id => new UserThatMatchPreferences(id)).ToList();
    }
}