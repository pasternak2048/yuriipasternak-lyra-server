using LYRA.Client.Abstractions;
using LYRA.Security.Signing;

namespace LYRA.Client.Touchpoints
{
	/// <summary>
	/// Resolves signing touchpoints from an in-memory list of configurations.
	/// </summary>
	public sealed class InMemoryTouchpointResolver : ITouchpointResolver
	{
		private readonly IReadOnlyList<TouchpointConfig> _touchpoints;

		public InMemoryTouchpointResolver(IEnumerable<TouchpointConfig> touchpoints)
		{
			_touchpoints = touchpoints.ToList();
		}

		/// <inheritdoc />
		public TouchpointBinding Resolve(GenericMetadata metadata, string? touchpointKey = null)
		{
			TouchpointConfig? config;

			if (!string.IsNullOrWhiteSpace(touchpointKey))
			{
				config = _touchpoints.FirstOrDefault(t => t.Key == touchpointKey);
			}
			else
			{
				config = _touchpoints.FirstOrDefault(t =>
					t.CallerSystemName == metadata.CallerSystemName &&
					t.TargetSystemName == metadata.TargetSystemName);
			}

			if (config is null)
				throw new InvalidOperationException($"Touchpoint not found (key: '{touchpointKey ?? "auto"}').");

			return new TouchpointBinding
			{
				Secret = config.Secret,
				SignatureType = config.SignatureType
			};
		}
	}
}
