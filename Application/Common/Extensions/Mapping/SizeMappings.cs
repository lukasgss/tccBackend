using Application.Common.DTOs;
using Domain.Enums;

namespace Application.Common.Extensions.Mapping;

public static class SizeMappings
{
    public static SizeResponse ToSizeResponse(this Size size)
    {
        return new SizeResponse(size, Enum.GetName(typeof(Size), size)!);
    }

    public static List<SizeResponse> ToListOfSizeResponse(this ICollection<Size>? sizes)
    {
        if (sizes is null)
        {
            return [];
        }

        return sizes.Select(size => new SizeResponse(size, Enum.GetName(typeof(Size), size)!))
            .ToList();
    }
}