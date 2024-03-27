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

		private static PackageTransformer _packageTransformer;

		#endregion

		#region Properties

		protected internal virtual PackageTransformer PackageTransformer
		{
			get
			{
				// ReSharper disable InvertIf
				if(_packageTransformer == null)
				{
					var fileSystemMock = new Mock<IFileSystem>();

					var directoryMock = new Mock<IDirectory>();
					directoryMock.Setup(directory => directory.Exists(It.IsAny<string>())).Returns(false);

					var fileMock = new Mock<IFile>();
					fileMock.Setup(file => file.Exists(It.IsAny<string>())).Returns(false);

					fileSystemMock.Setup(fileSystem => fileSystem.Directory).Returns(directoryMock.Object);
					fileSystemMock.Setup(fileSystem => fileSystem.File).Returns(fileMock.Object);

					_packageTransformer = new PackageTransformer(Mock.Of<IFileSearcher>(), fileSystemMock.Object, Mock.Of<IFileTransformerFactory>(), Mock.Of<ILoggerFactory>(), Mock.Of<IOptionsMonitor<TransformingOptions>>(), Mock.Of<IPackageHandlerLoader>());
				}
				// ReSharper restore InvertIf

				return _packageTransformer;
			}
		}

		#endregion

		#region Methods

		[Fact]
		public void Transform_WithSixParameters_IfTheDestinationParameterIsEmpty_ShouldThrowAnArgumentException()
		{
			Assert.Throws<ArgumentException>("destination", () => this.PackageTransformer.Transform(string.Empty, [], [], "Test", []));
		}

		[Fact]
		public void Transform_WithSixParameters_IfTheDestinationParameterIsNull_ShouldThrowAnArgumentNullException()
		{
			Assert.Throws<ArgumentNullException>("destination", () => this.PackageTransformer.Transform(null, [], [], "Test", []));
		}

		[Fact]
		public void Transform_WithSixParameters_IfTheDestinationParameterIsWhitespace_ShouldThrowAnArgumentException()
		{
			Assert.Throws<ArgumentException>("destination", () => this.PackageTransformer.Transform(" ", [], [], "Test", []));
		}

		[Fact]
		public void Transform_WithSixParameters_IfTheSourceParameterDoesNotExistAsFileSystemEntry_ShouldThrowAnArgumentException()
		{
			Assert.Throws<ArgumentException>("source", () => this.PackageTransformer.Transform("Test", [], [], "Test", []));
		}

		[Fact]
		public void Transform_WithSixParameters_IfTheSourceParameterIsEmpty_ShouldThrowAnArgumentException()
		{
			Assert.Throws<ArgumentException>("source", () => this.PackageTransformer.Transform("Test", [], [], string.Empty, []));
		}

		[Fact]
		public void Transform_WithSixParameters_IfTheSourceParameterIsNull_ShouldThrowAnArgumentNullException()
		{
			Assert.Throws<ArgumentNullException>("source", () => this.PackageTransformer.Transform("Test", [], [], null, []));
		}

		[Fact]
		public void Transform_WithSixParameters_IfTheSourceParameterIsWhitespace_ShouldThrowAnArgumentException()
		{
			Assert.Throws<ArgumentException>("source", () => this.PackageTransformer.Transform("Test", [], [], " ", []));
		}

		[Fact]
		public async Task ValidateFilePath_IfAllParametersAreNull_ShouldNotThrowAnException()
		{
			await Task.CompletedTask;

			this.PackageTransformer.ValidateFilePath(null, null, null);
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

			this.PackageTransformer.ValidateFilePath(null, null, filePath);
		}

		#endregion
	}
}