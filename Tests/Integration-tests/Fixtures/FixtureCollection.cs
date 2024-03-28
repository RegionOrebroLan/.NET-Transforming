namespace IntegrationTests.Fixtures
{
	[CollectionDefinition(Name)]
	public class FixtureCollection : ICollectionFixture<Fixture>
	{
		#region Fields

		public const string Name = "Global";

		#endregion
	}
}