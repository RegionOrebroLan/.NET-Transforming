using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RegionOrebroLan.Transforming;
using RegionOrebroLan.Transforming.IO;

namespace UnitTests
{
	[TestClass]
	public class PackageTransformerTest : BasicTest
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

					_packageTransformer = new PackageTransformer(Mock.Of<IFileSearcher>(), fileSystemMock.Object, Mock.Of<IFileTransformerFactory>(), Mock.Of<ILoggerFactory>(), Mock.Of<IPackageHandlerLoader>());
				}
				// ReSharper restore InvertIf

				return _packageTransformer;
			}
		}

		#endregion

		#region Methods

		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public void Transform_WithSixParameters_IfTheDestinationParameterIsEmpty_ShouldThrowAnArgumentException()
		{
			this.ValidateDestinationParameterException<ArgumentException>(() => { this.PackageTransformer.Transform(false, string.Empty, [], [], "Test", []); });
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void Transform_WithSixParameters_IfTheDestinationParameterIsNull_ShouldThrowAnArgumentNullException()
		{
			this.ValidateDestinationParameterException<ArgumentNullException>(() => { this.PackageTransformer.Transform(false, null, [], [], "Test", []); });
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public void Transform_WithSixParameters_IfTheDestinationParameterIsWhitespace_ShouldThrowAnArgumentException()
		{
			this.ValidateDestinationParameterException<ArgumentException>(() => { this.PackageTransformer.Transform(false, " ", [], [], "Test", []); });
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public void Transform_WithSixParameters_IfTheSourceParameterDoesNotExistAsFileSystemEntry_ShouldThrowAnArgumentException()
		{
			this.ValidateSourceParameterException<ArgumentException>(() => { this.PackageTransformer.Transform(false, "Test", [], [], "Test", []); });
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public void Transform_WithSixParameters_IfTheSourceParameterIsEmpty_ShouldThrowAnArgumentException()
		{
			this.ValidateSourceParameterException<ArgumentException>(() => { this.PackageTransformer.Transform(false, "Test", [], [], string.Empty, []); });
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void Transform_WithSixParameters_IfTheSourceParameterIsNull_ShouldThrowAnArgumentNullException()
		{
			this.ValidateSourceParameterException<ArgumentNullException>(() => { this.PackageTransformer.Transform(false, "Test", [], [], null, []); });
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public void Transform_WithSixParameters_IfTheSourceParameterIsWhitespace_ShouldThrowAnArgumentException()
		{
			this.ValidateSourceParameterException<ArgumentException>(() => { this.PackageTransformer.Transform(false, "Test", [], [], " ", []); });
		}

		[TestMethod]
		public async Task ValidateFilePath_IfAllParametersAreNull_ShouldNotThrowAnException()
		{
			await Task.CompletedTask;

			this.PackageTransformer.ValidateFilePath(null, null, null);
		}

		[TestMethod]
		public async Task ValidateFilePath_IfTheFilePathParameterIsAnEmptyString_ShouldNotThrowAnException()
		{
			await this.ValidateFilePathShouldNotThrowAnException(string.Empty);
		}

		[TestMethod]
		public async Task ValidateFilePath_IfTheFilePathParameterIsNull_ShouldNotThrowAnException()
		{
			await this.ValidateFilePathShouldNotThrowAnException(null);
		}

		[TestMethod]
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