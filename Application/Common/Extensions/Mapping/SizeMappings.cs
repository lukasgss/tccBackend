using Application.Common.DTOs;
using Domain.Enums;

namespace Application.Common.Extensions.Mapping;

public static class SizeMappings
{
    extension(Size size)
    {
        public SizeResponse ToSizeResponse()
        {
            return new SizeResponse(size, Enum.GetName(typeof(Size), size)!);
        }
    }

    extension(ICollection<Size>? sizes)
    {
        public List<SizeResponse> ToListOfSizeResponse()
        {
            if (sizes is null) return [];

            return sizes.Select(size => new SizeResponse(size, Enum.GetName(typeof(Size), size)!))
                .ToList();
        }
    }
}