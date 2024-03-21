namespace IntegrationTests
{
	public static class Global
	{
		#region Fields

		public static DirectoryInfo ProjectDirectory = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory).Parent!.Parent!.Parent!;

		#endregion
	}
}