using IntegrationTests.Fixtures;
using RegionOrebroLan.Transforming;
using Xunit;

namespace IntegrationTests
{
	public class FileTransformerFactoryTest(Fixture fixture) : IClassFixture<Fixture>
	{
		#region Fields

		private readonly Fixture _fixture = fixture ?? throw new ArgumentNullException(nameof(fixture));
		private static readonly string _resourcesPath = Path.Combine(Global.ProjectDirectory.FullName, "Resources", "FileTransformerFactoryTest");

		#endregion

		#region Methods

		[Fact]
		public void Create_IfTheSourceIsAJsonFile_ShouldReturnAJsonTransformer()
		{
			Assert.True(this._fixture.FileTransformerFactory.Create(GetPath("appsettings.json")) is JsonTransformer);
		}

		[Fact]
		public void Create_IfTheSourceIsAXmlFile_ShouldReturnAXmlTransformer()
		{
			Assert.True(this._fixture.FileTransformerFactory.Create(GetPath("Web.config")) is XmlTransformer);
		}

		[Fact]
		public void Create_IfTheSourceIsNeitherAJsonFileNorAXmlFile_ShouldThrowAnInvalidOperationException()
		{
			Assert.Throws<InvalidOperationException>(() => this._fixture.FileTransformerFactory.Create(GetPath("File.txt")));
		}

		[Fact]
		public void Create_IfTheSourceParameterDoesNotExistAsFile_ShouldThrowAnArgumentException()
		{
			Assert.Throws<ArgumentException>("source", () => this._fixture.FileTransformerFactory.Create(GetPath("Non-existing-file.txt")));
		}

		private static string GetPath(params string[] paths)
		{
			return Path.Combine(new[] { _resourcesPath }.Concat(paths).ToArray());
		}

		#endregion
	}
}