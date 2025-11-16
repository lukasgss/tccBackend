using Application.Queries.Colors.GetAll;
using Color = Domain.Entities.Color;

namespace Application.Common.Extensions.Mapping;

public static class ColorMappings
{
    extension(Color color)
    {
        public ColorResponse ToColorResponse()
        {
            return new ColorResponse(
                Id: color.Id,
                Name: color.Name,
                HexCode: color.HexCode
            );
        }
    }

    extension(ICollection<Color>? colors)
    {
        public List<ColorResponse> ToListOfColorResponse()
        {
            if (colors is null)
            {
                return [];
            }

            return colors.Select(color => new ColorResponse(
                    Id: color.Id,
                    Name: color.Name,
                    HexCode: color.HexCode
                )
            ).ToList();
        }
    }
}