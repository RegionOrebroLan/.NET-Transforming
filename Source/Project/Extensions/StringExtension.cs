using RegionOrebroLan.Transforming.Runtime;

namespace RegionOrebroLan.Transforming.Extensions
{
	public static class StringExtension
	{
		#region Methods

		public static string ResolveNewLines(this string value, IPlatform platform)
		{
			if(platform == null)
				throw new ArgumentNullException(nameof(platform));

			if(value != null && !platform.IsWindows)
				return value.Replace("\r\n", Environment.NewLine);

			return value;
		}

		#endregion
	}
}