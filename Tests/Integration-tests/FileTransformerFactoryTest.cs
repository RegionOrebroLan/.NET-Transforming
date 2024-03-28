using IntegrationTests.Fixtures;
using RegionOrebroLan.Transforming;
using Xunit;

namespace IntegrationTests
{
	[Collection(FixtureCollection.Name)]
	public class FileTransformerFactoryTest(Fixture fixture)
	{
		#region Fields

		private readonly Fixture _fixture = fixture ?? throw new ArgumentNullException(nameof(fixture));

		#endregion

		#region Methods

		[Fact]
		public void Create_IfTheSourceIsAJsonFile_ShouldReturnAJsonTransformer()
		{
			Assert.True(this._fixture.FileTransformerFactory.Create(this.GetResourcePath("appsettings.json")) is JsonTransformer);
		}

		[Fact]
		public void Create_IfTheSourceIsAXmlFile_ShouldReturnAXmlTransformer()
		{
			Assert.True(this._fixture.FileTransformerFactory.Create(this.GetResourcePath("Web.config")) is XmlTransformer);
		}

		[Fact]
		public void Create_IfTheSourceIsNeitherAJsonFileNorAXmlFile_ShouldThrowAnInvalidOperationException()
		{
			Assert.Throws<InvalidOperationException>(() => this._fixture.FileTransformerFactory.Create(this.GetResourcePath("File.txt")));
		}

		[Fact]
		public void Create_IfTheSourceParameterDoesNotExistAsFile_ShouldThrowAnArgumentException()
		{
			Assert.Throws<ArgumentException>("source", () => this._fixture.FileTransformerFactory.Create(this.GetResourcePath("Non-existing-file.txt")));
		}

		private string GetResourcePath(params string[] paths)
		{
			return this._fixture.GetResourcePath(new[] { "FileTransformerFactoryTest" }.Concat(paths).ToArray());
		}

		#endregion
	}
}