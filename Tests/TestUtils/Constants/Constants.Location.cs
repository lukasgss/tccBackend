using Domain.Entities;

namespace Tests.TestUtils.Constants;

public static partial class Constants
{
	public static class LocationData
	{
		public static readonly State State = new(1, "São Paulo");
		public static readonly City City = new(1, "São Paulo");
	}
}