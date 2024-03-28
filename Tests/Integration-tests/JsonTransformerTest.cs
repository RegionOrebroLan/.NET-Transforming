using IntegrationTests.Fixtures;
using Xunit;

namespace IntegrationTests
{
	[Collection(FixtureCollection.Name)]
	public class JsonTransformerTest(Fixture fixture)
	{
		#region Fields

		private readonly Fixture _fixture = fixture ?? throw new ArgumentNullException(nameof(fixture));

		#endregion

		#region Methods

		private string GetOutputPath(params string[] paths)
		{
			return this._fixture.GetOutputPath(ResolvePaths(paths));
		}

		private string GetResourcePath(params string[] paths)
		{
			return this._fixture.GetResourcePath(ResolvePaths(paths));
		}

		private static string[] ResolvePaths(params string[] paths)
		{
			return new[] { "JsonTransformerTest" }.Concat(paths).ToArray();
		}

		[Fact]
		public void Transform_IfTheTransformationContentIsEmpty_ShouldTransformCorrectly()
		{
			const string fileName = "appsettings.json";
			var destination = this.GetOutputPath(fileName);
			var source = this.GetResourcePath(fileName);
			var transformation = this.GetResourcePath("appsettings.No-Transformation.json");

			this._fixture.JsonTransformer.Transform(destination, source, transformation);

			var expectedContent = File.ReadAllText(this.GetResourcePath("appsettings.No-Transformation.Expected.json"));
			var actualContent = File.ReadAllText(destination);

			Assert.Equal(expectedContent, actualContent);
		}

		[Fact]
		public void Transform_ShouldTransformCorrectly()
		{
			const string fileName = "appsettings.json";
			var destination = this.GetOutputPath(fileName);
			var source = this.GetResourcePath(fileName);
			var transformation = this.GetResourcePath("appsettings.Transformation.json");

			this._fixture.JsonTransformer.Transform(destination, source, transformation);

			var expectedContent = File.ReadAllText(this.GetResourcePath("appsettings.Expected.json"));
			var actualContent = File.ReadAllText(destination);

			Assert.Equal(expectedContent, actualContent);
		}

		#endregion
	}
}