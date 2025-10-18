using Application.Queries.Colors.GetAll;
using Color = Domain.Entities.Color;

namespace Application.Common.Extensions.Mapping;

public static class ColorMappings
{
    public static ColorResponse ToColorResponse(this Color color)
    {
        return new ColorResponse(
            Id: color.Id,
            Name: color.Name,
            HexCode: color.HexCode
        );
    }

    public static List<ColorResponse> ToListOfColorResponse(this ICollection<Color>? colors)
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