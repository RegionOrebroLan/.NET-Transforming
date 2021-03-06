﻿using System;
using System.IO.Abstractions;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RegionOrebroLan.IO;

namespace RegionOrebroLan.Transforming.UnitTests
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

					var directoryBaseMock = new Mock<DirectoryBase>(fileSystemMock.Object) {CallBase = true};
					directoryBaseMock.Setup(directoryBase => directoryBase.Exists(It.IsAny<string>())).Returns(false);

					var fileBaseMock = new Mock<FileBase>(fileSystemMock.Object) {CallBase = true};
					fileBaseMock.Setup(fileBase => fileBase.Exists(It.IsAny<string>())).Returns(false);

					fileSystemMock.Setup(fileSystem => fileSystem.Directory).Returns(directoryBaseMock.Object);
					fileSystemMock.Setup(fileSystem => fileSystem.File).Returns(fileBaseMock.Object);

					_packageTransformer = new PackageTransformer(Mock.Of<IFileSystemEntryMatcher>(), fileSystemMock.Object, Mock.Of<IFileTransformerFactory>(), Mock.Of<IPackageHandlerLoader>());
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

		#endregion
	}
}