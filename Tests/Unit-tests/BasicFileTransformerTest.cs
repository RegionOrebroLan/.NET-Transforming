using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RegionOrebroLan.Transforming;
using RegionOrebroLan.Transforming.Configuration;
using RegionOrebroLan.Transforming.IO;

namespace UnitTests
{
	[TestClass]
	public class BasicFileTransformerTest : BasicTest
	{
		#region Fields

		private static BasicFileTransformer _basicFileTransformer;
		private const string _existingPath = "Exists";

		#endregion

		#region Properties

		protected internal virtual BasicFileTransformer BasicFileTransformer => _basicFileTransformer ??= CreateBasicFileTransformer(this.ExistingPath);
		protected internal virtual string ExistingPath => _existingPath;

		#endregion

		#region Methods

		private static BasicFileTransformer CreateBasicFileTransformer(string existingPath)
		{
			var fileSystemMock = new Mock<IFileSystem>();

			var fileMock = new Mock<IFile>();
			fileMock.Setup(file => file.Exists(It.IsAny<string>())).Returns<string>(path => string.Equals(path, existingPath, StringComparison.OrdinalIgnoreCase));

			fileSystemMock.Setup(fileSystem => fileSystem.File).Returns(fileMock.Object);

			return new Mock<BasicFileTransformer>(fileSystemMock.Object, Mock.Of<ILoggerFactory>(), Mock.Of<IOptionsMonitor<TransformingOptions>>()) { CallBase = true }.Object;
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public void Transform_IfTheDestinationParameterIsEmpty_ShouldThrowAnArgumentException()
		{
			this.ValidateDestinationParameterException<ArgumentException>(() => { this.BasicFileTransformer.Transform(string.Empty, "Test", "Test"); });
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void Transform_IfTheDestinationParameterIsNull_ShouldThrowAnArgumentException()
		{
			this.ValidateDestinationParameterException<ArgumentNullException>(() => { this.BasicFileTransformer.Transform(null, "Test", "Test"); });
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public void Transform_IfTheDestinationParameterIsWhitespace_ShouldThrowAnArgumentException()
		{
			this.ValidateDestinationParameterException<ArgumentException>(() => { this.BasicFileTransformer.Transform(" ", "Test", "Test"); });
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public void Transform_IfTheSourceParameterDoesNotExistAsFile_ShouldThrowAnArgumentException()
		{
			this.ValidateSourceParameterException<ArgumentException>(() => { this.BasicFileTransformer.Transform("Test", "Test", "Test"); });
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public void Transform_IfTheSourceParameterIsEmpty_ShouldThrowAnArgumentException()
		{
			this.ValidateSourceParameterException<ArgumentException>(() => { this.BasicFileTransformer.Transform("Test", string.Empty, "Test"); });
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void Transform_IfTheSourceParameterIsNull_ShouldThrowAnArgumentException()
		{
			this.ValidateSourceParameterException<ArgumentNullException>(() => { this.BasicFileTransformer.Transform("Test", null, "Test"); });
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public void Transform_IfTheSourceParameterIsWhitespace_ShouldThrowAnArgumentException()
		{
			this.ValidateSourceParameterException<ArgumentException>(() => { this.BasicFileTransformer.Transform("Test", " ", "Test"); });
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public void Transform_IfTheTransformationParameterDoesNotExistAsFile_ShouldThrowAnArgumentException()
		{
			this.ValidateTransformationParameterException<ArgumentException>(() => { this.BasicFileTransformer.Transform("Test", this.ExistingPath, "Test"); });
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public void Transform_IfTheTransformationParameterIsEmpty_ShouldThrowAnArgumentException()
		{
			this.ValidateTransformationParameterException<ArgumentException>(() => { this.BasicFileTransformer.Transform("Test", this.ExistingPath, string.Empty); });
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void Transform_IfTheTransformationParameterIsNull_ShouldThrowAnArgumentException()
		{
			this.ValidateTransformationParameterException<ArgumentNullException>(() => { this.BasicFileTransformer.Transform("Test", this.ExistingPath, null); });
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public void Transform_IfTheTransformationParameterIsWhitespace_ShouldThrowAnArgumentException()
		{
			this.ValidateTransformationParameterException<ArgumentException>(() => { this.BasicFileTransformer.Transform("Test", this.ExistingPath, " "); });
		}

		#endregion
	}
}