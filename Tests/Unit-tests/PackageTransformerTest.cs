using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using RegionOrebroLan.Transforming;
using RegionOrebroLan.Transforming.Configuration;
using RegionOrebroLan.Transforming.IO;

namespace UnitTests
{
	public class PackageTransformerTest
	{
		#region Fields

		private static readonly PackageTransformer _packageTransformer = CreatePackageTransformer();

		#endregion

		#region Methods

		private static PackageTransformer CreatePackageTransformer()
		{
			var fileSystemMock = new Mock<IFileSystem>();

			var directoryMock = new Mock<IDirectory>();
			directoryMock.Setup(directory => directory.Exists(It.IsAny<string>())).Returns(false);

			var fileMock = new Mock<IFile>();
			fileMock.Setup(file => file.Exists(It.IsAny<string>())).Returns(false);

			fileSystemMock.Setup(fileSystem => fileSystem.Directory).Returns(directoryMock.Object);
			fileSystemMock.Setup(fileSystem => fileSystem.File).Returns(fileMock.Object);

			return new PackageTransformer(Mock.Of<IFileSearcher>(), fileSystemMock.Object, Mock.Of<IFileTransformerFactory>(), Mock.Of<ILoggerFactory>(), Mock.Of<IOptionsMonitor<TransformingOptions>>(), Mock.Of<IPackageHandlerLoader>());
		}

		[Fact]
		public void Transform_IfTheDestinationParameterIsEmpty_ShouldThrowAnArgumentException()
		{
			Assert.Throws<ArgumentException>("destination", () => _packageTransformer.Transform(string.Empty, [], [], "Test", []));
		}

		[Fact]
		public void Transform_IfTheDestinationParameterIsNull_ShouldThrowAnArgumentNullException()
		{
			Assert.Throws<ArgumentNullException>("destination", () => _packageTransformer.Transform(null, [], [], "Test", []));
		}

		[Fact]
		public void Transform_IfTheDestinationParameterIsWhitespace_ShouldThrowAnArgumentException()
		{
			Assert.Throws<ArgumentException>("destination", () => _packageTransformer.Transform(" ", [], [], "Test", []));
		}

		[Fact]
		public void Transform_IfTheSourceParameterDoesNotExistAsFileSystemEntry_ShouldThrowAnArgumentException()
		{
			Assert.Throws<ArgumentException>("source", () => _packageTransformer.Transform("Test", [], [], "Test", []));
		}

		[Fact]
		public void Transform_IfTheSourceParameterIsEmpty_ShouldThrowAnArgumentException()
		{
			Assert.Throws<ArgumentException>("source", () => _packageTransformer.Transform("Test", [], [], string.Empty, []));
		}

		[Fact]
		public void Transform_IfTheSourceParameterIsNull_ShouldThrowAnArgumentNullException()
		{
			Assert.Throws<ArgumentNullException>("source", () => _packageTransformer.Transform("Test", [], [], null, []));
		}

		[Fact]
		public void Transform_IfTheSourceParameterIsWhitespace_ShouldThrowAnArgumentException()
		{
			Assert.Throws<ArgumentException>("source", () => _packageTransformer.Transform("Test", [], [], " ", []));
		}

		[Fact]
		public async Task ValidateFilePath_IfAllParametersAreNull_ShouldNotThrowAnException()
		{
			await Task.CompletedTask;

			_packageTransformer.ValidateFilePath(null, null, null);
		}

		[Fact]
		public async Task ValidateFilePath_IfTheFilePathParameterIsAnEmptyString_ShouldNotThrowAnException()
		{
			await this.ValidateFilePathShouldNotThrowAnException(string.Empty);
		}

		[Fact]
		public async Task ValidateFilePath_IfTheFilePathParameterIsNull_ShouldNotThrowAnException()
		{
			await this.ValidateFilePathShouldNotThrowAnException(null);
		}

		[Fact]
		public async Task ValidateFilePath_IfTheFilePathParameterIsOnlyWhitespaces_ShouldNotThrowAnException()
		{
			await this.ValidateFilePathShouldNotThrowAnException("    ");
		}

		protected internal virtual async Task ValidateFilePathShouldNotThrowAnException(string filePath)
		{
			await Task.CompletedTask;

			_packageTransformer.ValidateFilePath(null, null, filePath);
		}

		#endregion
	}
}