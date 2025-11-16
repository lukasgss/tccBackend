using Application.Common.Interfaces.FrontendDropdownData;
using Domain.Entities;

namespace Application.Common.Extensions.Mapping;

public static class LocalizationMappings
{
    extension(State state)
    {
        private DropdownDataResponse<string> ToDropdownResponse()
        {
            return new DropdownDataResponse<string>
            {
                Label = state.Name,
                Value = state.Id.ToString()
            };
        }
    }

    extension(IEnumerable<State> states)
    {
        public List<DropdownDataResponse<string>> ToListOfDropdownResponse()
        {
            return states.Select(state => state.ToDropdownResponse()).ToList();
        }
    }

    extension(City city)
    {
        private DropdownDataResponse<string> ToDropdownResponse()
        {
            return new DropdownDataResponse<string>
            {
                Label = city.Name,
                Value = city.Id.ToString()
            };
        }
    }

    extension(IEnumerable<City> cities)
    {
        public List<DropdownDataResponse<string>> ToListOfDropdownResponse()
        {
            return cities.Select(state => state.ToDropdownResponse()).ToList();
        }
    }
}