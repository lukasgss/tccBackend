using Application.Common.Interfaces.FrontendDropdownData;
using Domain.Entities;

namespace Application.Common.Extensions.Mapping;

public static class LocalizationMappings
{
    private static DropdownDataResponse<string> ToDropdownResponse(this State state)
    {
        return new DropdownDataResponse<string>()
        {
            Label = state.Name,
            Value = state.Id.ToString()
        };
    }
    
    public static List<DropdownDataResponse<string>> ToListOfDropdownResponse(this IEnumerable<State> states)
    {
        return states.Select(state => state.ToDropdownResponse()).ToList();
    }
    
    private static DropdownDataResponse<string> ToDropdownResponse(this City city)
    {
        return new DropdownDataResponse<string>()
        {
            Label = city.Name,
            Value = city.Id.ToString()
        };
    }
    
    public static List<DropdownDataResponse<string>> ToListOfDropdownResponse(this IEnumerable<City> city)
    {
        return city.Select(state => state.ToDropdownResponse()).ToList();
    }
}