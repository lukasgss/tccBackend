using Domain.Entities.Alerts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public sealed class FoundAnimalAlertConfiguration : IEntityTypeConfiguration<FoundAnimalAlert>
{
	public void Configure(EntityTypeBuilder<FoundAnimalAlert> builder)
	{
		builder.Property(p => p.Name)
			.HasMaxLength(255);

		builder.Property(p => p.Description)
			.HasMaxLength(500);

		builder.Property(p => p.Neighborhood)
			.HasMaxLength(100);

		builder.HasIndex(p => p.Gender);
		builder.HasIndex(p => p.Age);
		builder.HasIndex(p => p.Size);
	}
}