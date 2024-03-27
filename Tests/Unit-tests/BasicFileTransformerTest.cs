using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using RegionOrebroLan.Transforming;
using RegionOrebroLan.Transforming.Configuration;
using RegionOrebroLan.Transforming.IO;

namespace UnitTests
{
	public class BasicFileTransformerTest
	{
		#region Fields

		private static readonly BasicFileTransformer _basicFileTransformer = CreateBasicFileTransformer();
		private const string _existingPath = "/existing-path";

		#endregion

		#region Methods

		private static BasicFileTransformer CreateBasicFileTransformer()
		{
			var fileSystemMock = new Mock<IFileSystem>();

			var fileMock = new Mock<IFile>();
			fileMock.Setup(file => file.Exists(It.IsAny<string>())).Returns<string>(path => string.Equals(path, _existingPath, StringComparison.OrdinalIgnoreCase));

			fileSystemMock.Setup(fileSystem => fileSystem.File).Returns(fileMock.Object);

			return new Mock<BasicFileTransformer>(fileSystemMock.Object, Mock.Of<ILoggerFactory>(), Mock.Of<IOptionsMonitor<TransformingOptions>>()) { CallBase = true }.Object;
		}

		[Fact]
		public void Transform_IfTheDestinationParameterIsEmpty_ShouldThrowAnArgumentException()
		{
			Assert.Throws<ArgumentException>("destination", () => _basicFileTransformer.Transform(string.Empty, "Test", "Test"));
		}

		[Fact]
		public void Transform_IfTheDestinationParameterIsNull_ShouldThrowAnArgumentNullException()
		{
			Assert.Throws<ArgumentNullException>("destination", () => _basicFileTransformer.Transform(null, "Test", "Test"));
		}

		[Fact]
		public void Transform_IfTheDestinationParameterIsWhitespace_ShouldThrowAnArgumentException()
		{
			Assert.Throws<ArgumentException>("destination", () => _basicFileTransformer.Transform(" ", "Test", "Test"));
		}

		[Fact]
		public void Transform_IfTheSourceParameterDoesNotExistAsFile_ShouldThrowAnArgumentException()
		{
			Assert.Throws<ArgumentException>("source", () => _basicFileTransformer.Transform("Test", "Test", "Test"));
		}

		[Fact]
		public void Transform_IfTheSourceParameterIsEmpty_ShouldThrowAnArgumentException()
		{
			Assert.Throws<ArgumentException>("source", () => _basicFileTransformer.Transform("Test", string.Empty, "Test"));
		}

		[Fact]
		public void Transform_IfTheSourceParameterIsNull_ShouldThrowAnArgumentNullException()
		{
			Assert.Throws<ArgumentNullException>("source", () => _basicFileTransformer.Transform("Test", null, "Test"));
		}

		[Fact]
		public void Transform_IfTheSourceParameterIsWhitespace_ShouldThrowAnArgumentException()
		{
			Assert.Throws<ArgumentException>("source", () => _basicFileTransformer.Transform("Test", " ", "Test"));
		}

		[Fact]
		public void Transform_IfTheTransformationParameterDoesNotExistAsFile_ShouldThrowAnArgumentException()
		{
			Assert.Throws<ArgumentException>("transformation", () => _basicFileTransformer.Transform("Test", _existingPath, "Test"));
		}

		[Fact]
		public void Transform_IfTheTransformationParameterIsEmpty_ShouldThrowAnArgumentException()
		{
			Assert.Throws<ArgumentException>("transformation", () => _basicFileTransformer.Transform("Test", _existingPath, string.Empty));
		}

		[Fact]
		public void Transform_IfTheTransformationParameterIsNull_ShouldThrowAnArgumentNullException()
		{
			Assert.Throws<ArgumentNullException>("transformation", () => _basicFileTransformer.Transform("Test", _existingPath, null));
		}

		[Fact]
		public void Transform_IfTheTransformationParameterIsWhitespace_ShouldThrowAnArgumentException()
		{
			Assert.Throws<ArgumentException>("transformation", () => _basicFileTransformer.Transform("Test", _existingPath, " "));
		}

		#endregion
	}
}