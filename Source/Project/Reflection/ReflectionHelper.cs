namespace RegionOrebroLan.Transforming.Reflection
{
	public static class ReflectionHelper
	{
		#region Methods

		public static string ResolveFieldName(string name, string dotNetCoreNamePrefix = "_")
		{
			var prefix = Environment.Version.Major == 4 ? string.Empty : dotNetCoreNamePrefix;

			return $"{prefix}{name}";
		}

		#endregion
	}
}