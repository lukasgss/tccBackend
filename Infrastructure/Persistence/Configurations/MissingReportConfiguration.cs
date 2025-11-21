using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public sealed class MissingReportConfiguration : IEntityTypeConfiguration<MissingAnimalReport>
{
	public void Configure(EntityTypeBuilder<MissingAnimalReport> builder)
	{
		builder.Property(p => p.Reason)
			.HasMaxLength(100);

		builder.Property(p => p.RejectedReason)
			.HasMaxLength(100);
	}
}