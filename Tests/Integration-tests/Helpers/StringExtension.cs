namespace IntegrationTests.Helpers
{
	public static class StringExtension
	{
		#region Methods

		public static bool HasUnixLineBreaks(this string value)
		{
			return value.NumberOfUnixLineBreaks() > 0;
		}

		public static bool HasWindowsLineBreaks(this string value)
		{
			return value.NumberOfWindowsLineBreaks() > 0;
		}

		public static int NumberOfUnixLineBreaks(this string value)
		{
			value = (value ?? string.Empty).Replace("\r\n", "\a");

			return value.Split('\n').Length - 1;
		}

		public static int NumberOfWindowsLineBreaks(this string value)
		{
			const char separator = '\a';

			value = (value ?? string.Empty).Replace("\r\n", separator.ToString());

			return value.Split(separator).Length - 1;
		}

		#endregion
	}
}