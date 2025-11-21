using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public sealed class PetConfigurations : IEntityTypeConfiguration<Pet>
{
	public void Configure(EntityTypeBuilder<Pet> builder)
	{
		builder.Property(p => p.Name)
			.IsRequired()
			.HasMaxLength(255);

		builder.HasIndex(p => p.Gender);
		builder.HasIndex(p => p.Age);
		builder.HasIndex(p => p.Size);
	}
}