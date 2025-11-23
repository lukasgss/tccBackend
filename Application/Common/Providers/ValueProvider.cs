using System.Diagnostics.CodeAnalysis;
using Application.Common.Interfaces.Providers;

namespace Application.Common.Providers;

[ExcludeFromCodeCoverage]
public sealed class ValueProvider : IValueProvider
{
	public DateTime Now() => DateTime.Now;
	public DateTime UtcNow() => DateTime.UtcNow;
	public DateOnly DateOnlyNow() => DateOnly.FromDateTime(DateTime.Now);
	public Guid NewGuid() => Guid.NewGuid();
}