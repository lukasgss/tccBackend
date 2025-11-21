using Domain.Entities.Alerts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public sealed class MissingAlertConfiguration : IEntityTypeConfiguration<MissingAlert>
{
	public void Configure(EntityTypeBuilder<MissingAlert> builder)
	{
		builder.Property(p => p.Description)
			.HasMaxLength(1000);

		builder.Property(p => p.Neighborhood)
			.HasMaxLength(100);

		builder.HasIndex(p => p.Location);
	}
}