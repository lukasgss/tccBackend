using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Domain.Enums;
using Domain.ValueObjects;

namespace Domain.Entities;

public class Pet
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    [Required, EnumDataType(typeof(Gender))]
    public Gender Gender { get; set; }

    public bool? IsCastrated { get; set; }
    public bool? IsVaccinated { get; set; }
    public bool? IsNegativeToFivFelv { get; set; }
    public bool? IsNegativeToLeishmaniasis { get; set; }

    [Required, EnumDataType(typeof(Age))]
    public required Age Age { get; set; }

    [Required, EnumDataType(typeof(Size))]
    public required Size Size { get; set; }

    [Required, ForeignKey("UserId")]
    public virtual User Owner { get; set; } = null!;

    public Guid UserId { get; set; }

    [ForeignKey("BreedId")]
    public Breed Breed { get; set; } = null!;

    public int BreedId { get; set; }

    [ForeignKey("SpeciesId")]
    public virtual Species Species { get; set; } = null!;

    public int SpeciesId { get; set; }

    public virtual ICollection<Color> Colors { get; set; } = null!;
    public virtual List<PetImage> Images { get; set; } = null!;
}