using System;
using System.IO.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace RegionOrebroLan.Transforming.UnitTests
{
	[TestClass]
	public class BasicFileTransformerTest : BasicTest
	{
		#region Fields

		private static BasicFileTransformer _basicFileTransformer;
		private const string _existingPath = "Exists";

		#endregion

		#region Properties

		protected internal virtual BasicFileTransformer BasicFileTransformer
		{
			get
			{
				// ReSharper disable InvertIf
				if(_basicFileTransformer == null)
				{
					var fileSystemMock = new Mock<IFileSystem>();

					var fileBaseMock = new Mock<FileBase>(fileSystemMock.Object) {CallBase = true};
					fileBaseMock.Setup(fileBase => fileBase.Exists(It.IsAny<string>())).Returns<string>(path => string.Equals(path, this.ExistingPath, StringComparison.OrdinalIgnoreCase));

					fileSystemMock.Setup(fileSystem => fileSystem.File).Returns(fileBaseMock.Object);

					_basicFileTransformer = new Mock<BasicFileTransformer>(fileSystemMock.Object) {CallBase = true}.Object;
				}
				// ReSharper restore InvertIf

				return _basicFileTransformer;
			}
		}

		protected internal virtual string ExistingPath => _existingPath;

		#endregion

		#region Methods

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