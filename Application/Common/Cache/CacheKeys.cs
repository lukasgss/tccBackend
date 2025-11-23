using System.Diagnostics.CodeAnalysis;

namespace Application.Common.Cache;

[ExcludeFromCodeCoverage]
public static class CacheKeys
{
    public const string Colors = "Colors";
    public const string Ages = "Ages";
    public const string Genders = "Genders";
    public const string Species = "Species";
    public const string Sizes = "Sizes";
    public const string AllStates = "AllStates";
    private const string Breeds = "Breeds";
    private const string Cities = "Cities";

    public static string BreedOfSpecies(int speciesId) => $"{Breeds}-{speciesId}";
    public static string CitiesOfState(int stateId) => $"{Cities}-{stateId}";
}