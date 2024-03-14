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

					_packageTransformer = new PackageTransformer(Mock.Of<IFileSearcher>(), fileSystemMock.Object, Mock.Of<IFileTransformerFactory>(), Mock.Of<IPackageHandlerLoader>());
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
			this.ValidateDestinationParameterException<ArgumentException>(() => { this.PackageTransformer.Transform(false, string.Empty, Enumerable.Empty<string>(), Enumerable.Empty<string>(), "Test", Enumerable.Empty<string>()); });
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void Transform_WithSixParameters_IfTheDestinationParameterIsNull_ShouldThrowAnArgumentNullException()
		{
			this.ValidateDestinationParameterException<ArgumentNullException>(() => { this.PackageTransformer.Transform(false, null, Enumerable.Empty<string>(), Enumerable.Empty<string>(), "Test", Enumerable.Empty<string>()); });
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public void Transform_WithSixParameters_IfTheDestinationParameterIsWhitespace_ShouldThrowAnArgumentException()
		{
			this.ValidateDestinationParameterException<ArgumentException>(() => { this.PackageTransformer.Transform(false, " ", Enumerable.Empty<string>(), Enumerable.Empty<string>(), "Test", Enumerable.Empty<string>()); });
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public void Transform_WithSixParameters_IfTheSourceParameterDoesNotExistAsFileSystemEntry_ShouldThrowAnArgumentException()
		{
			this.ValidateSourceParameterException<ArgumentException>(() => { this.PackageTransformer.Transform(false, "Test", Enumerable.Empty<string>(), Enumerable.Empty<string>(), "Test", Enumerable.Empty<string>()); });
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public void Transform_WithSixParameters_IfTheSourceParameterIsEmpty_ShouldThrowAnArgumentException()
		{
			this.ValidateSourceParameterException<ArgumentException>(() => { this.PackageTransformer.Transform(false, "Test", Enumerable.Empty<string>(), Enumerable.Empty<string>(), string.Empty, Enumerable.Empty<string>()); });
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void Transform_WithSixParameters_IfTheSourceParameterIsNull_ShouldThrowAnArgumentNullException()
		{
			this.ValidateSourceParameterException<ArgumentNullException>(() => { this.PackageTransformer.Transform(false, "Test", Enumerable.Empty<string>(), Enumerable.Empty<string>(), null, Enumerable.Empty<string>()); });
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public void Transform_WithSixParameters_IfTheSourceParameterIsWhitespace_ShouldThrowAnArgumentException()
		{
			this.ValidateSourceParameterException<ArgumentException>(() => { this.PackageTransformer.Transform(false, "Test", Enumerable.Empty<string>(), Enumerable.Empty<string>(), " ", Enumerable.Empty<string>()); });
		}

		[TestMethod]
		public void ValidateFilePath_IfAllParametersAreNull_ShouldNotThrowAnException()
		{
			this.PackageTransformer.ValidateFilePath(null, null, null);
		}

		[TestMethod]
		public void ValidateFilePath_IfTheFilePathParameterIsAnEmptyString_ShouldNotThrowAnException()
		{
			this.ValidateFilePathShouldNotThrowAnException(string.Empty);
		}

		[TestMethod]
		public void ValidateFilePath_IfTheFilePathParameterIsNull_ShouldNotThrowAnException()
		{
			this.ValidateFilePathShouldNotThrowAnException(null);
		}

		[TestMethod]
		public void ValidateFilePath_IfTheFilePathParameterIsOnlyWhitespaces_ShouldNotThrowAnException()
		{
			this.ValidateFilePathShouldNotThrowAnException("    ");
		}

		protected internal virtual void ValidateFilePathShouldNotThrowAnException(string filePath)
		{
			this.PackageTransformer.ValidateFilePath(null, null, filePath);
			this.PackageTransformer.ValidateFilePath(null, string.Empty, filePath);
			this.PackageTransformer.ValidateFilePath(string.Empty, null, filePath);
			this.PackageTransformer.ValidateFilePath(string.Empty, string.Empty, filePath);
			this.PackageTransformer.ValidateFilePath(null, "Test", filePath);
			this.PackageTransformer.ValidateFilePath("Test", null, filePath);
			this.PackageTransformer.ValidateFilePath("Test", "Test", filePath);
		}

		#endregion
	}
}