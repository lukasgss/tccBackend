using Domain.Entities;

namespace Infrastructure.Persistence.DataSeed;

public static class SeedColors
{
    public static List<Color> Seed()
    {
        return
        [
            new Color() { Id = 1, Name = "Branco", HexCode = "#FFFFFF" },
            new Color() { Id = 2, Name = "Preto", HexCode = "#000000" },
            new Color() { Id = 3, Name = "Caramelo", HexCode = "#C68E17" },
            new Color() { Id = 4, Name = "Marrom", HexCode = "#8B4513" },
            new Color() { Id = 5, Name = "Cinza", HexCode = "#808080" },
            new Color() { Id = 6, Name = "Laranja", HexCode = "#FFA500" },
            new Color() { Id = 7, Name = "Creme", HexCode = "#FFFDD0" },
            new Color() { Id = 8, Name = "Chocolate", HexCode = "#7B3F00" },
            new Color() { Id = 9, Name = "Fulvo", HexCode = "#DEB887" },
            new Color() { Id = 10, Name = "Ruivo", HexCode = "#CD5C5C" },
            new Color() { Id = 11, Name = "Bege", HexCode = "#E4D5B7" },
            new Color() { Id = 12, Name = "Tigrado", HexCode = "#8B4513" },
        ];
    }
}