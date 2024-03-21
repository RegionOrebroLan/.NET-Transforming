using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RegionOrebroLan.Transforming;
using RegionOrebroLan.Transforming.IO;
using RegionOrebroLan.Transforming.Runtime;

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

		protected internal virtual BasicFileTransformer BasicFileTransformer => _basicFileTransformer ??= CreateBasicFileTransformer(this.ExistingPath, true);
		protected internal virtual string ExistingPath => _existingPath;

		#endregion

		#region Methods

		private static BasicFileTransformer CreateBasicFileTransformer(bool isWindows)
		{
			return CreateBasicFileTransformer(_existingPath, isWindows);
		}

		private static BasicFileTransformer CreateBasicFileTransformer(string existingPath, bool isWindows)
		{
			var fileSystemMock = new Mock<IFileSystem>();

			var fileMock = new Mock<IFile>();
			fileMock.Setup(file => file.Exists(It.IsAny<string>())).Returns<string>(path => string.Equals(path, existingPath, StringComparison.OrdinalIgnoreCase));

			fileSystemMock.Setup(fileSystem => fileSystem.File).Returns(fileMock.Object);

			var platformMock = new Mock<IPlatform>();
			platformMock.Setup(runtime => runtime.IsWindows).Returns(isWindows);

			return new Mock<BasicFileTransformer>(fileSystemMock.Object, Mock.Of<ILoggerFactory>(), platformMock.Object) { CallBase = true }.Object;
		}

		[TestMethod]
		public void ResolveAvoidByteOrderMark_IfTheAvoidByteOrderMarkParameterIsFalse_And_ThePlatformIsNotWindows_ShouldReturnFalse()
		{
			var basicFileTransformer = CreateBasicFileTransformer(false);
			Assert.IsFalse(basicFileTransformer.ResolveAvoidByteOrderMark(false));
		}

		[TestMethod]
		public void ResolveAvoidByteOrderMark_IfTheAvoidByteOrderMarkParameterIsFalse_And_ThePlatformIsWindows_ShouldReturnFalse()
		{
			var basicFileTransformer = CreateBasicFileTransformer(true);
			Assert.IsFalse(basicFileTransformer.ResolveAvoidByteOrderMark(false));
		}

		[TestMethod]
		public void ResolveAvoidByteOrderMark_IfTheAvoidByteOrderMarkParameterIsNull_And_ThePlatformIsNotWindows_ShouldReturnTrue()
		{
			var basicFileTransformer = CreateBasicFileTransformer(false);
			Assert.IsTrue(basicFileTransformer.ResolveAvoidByteOrderMark(null));
		}

		[TestMethod]
		public void ResolveAvoidByteOrderMark_IfTheAvoidByteOrderMarkParameterIsNull_And_ThePlatformIsWindows_ShouldReturnFalse()
		{
			var basicFileTransformer = CreateBasicFileTransformer(true);
			Assert.IsFalse(basicFileTransformer.ResolveAvoidByteOrderMark(null));
		}

		[TestMethod]
		public void ResolveAvoidByteOrderMark_IfTheAvoidByteOrderMarkParameterIsTrue_And_ThePlatformIsNotWindows_ShouldReturnTrue()
		{
			var basicFileTransformer = CreateBasicFileTransformer(false);
			Assert.IsTrue(basicFileTransformer.ResolveAvoidByteOrderMark(true));
		}

		[TestMethod]
		public void ResolveAvoidByteOrderMark_IfTheAvoidByteOrderMarkParameterIsTrue_And_ThePlatformIsWindows_ShouldReturnTrue()
		{
			var basicFileTransformer = CreateBasicFileTransformer(true);
			Assert.IsTrue(basicFileTransformer.ResolveAvoidByteOrderMark(true));
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