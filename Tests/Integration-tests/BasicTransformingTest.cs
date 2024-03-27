namespace IntegrationTests
{
	public abstract class BasicTransformingTest : BasicTest
	{
		#region Properties

		protected internal override string ProjectRelativeTestResourceDirectoryPath => Path.Combine("Resources", "_old");

		#endregion
	}
}