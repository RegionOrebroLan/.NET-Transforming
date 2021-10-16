using System;
using System.IO.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RegionOrebroLan.Transforming;

namespace UnitTests
{
	[TestClass]
	public class FileTransformerFactoryTest
	{
		#region Fields

		private static FileTransformerFactory _fileTransformerFactory;

		#endregion

		#region Properties

		protected internal virtual FileTransformerFactory FileTransformerFactory => _fileTransformerFactory ?? (_fileTransformerFactory = new FileTransformerFactory(Mock.Of<IFileSystem>()));

		#endregion

		#region Methods

		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public void Create_IfTheSourceParameterIsEmpty_ShouldThrowAnArgumentException()
		{
			this.FileTransformerFactory.Create(string.Empty);
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void Create_IfTheSourceParameterIsNull_ShouldThrowAnArgumentNullException()
		{
			this.FileTransformerFactory.Create(null);
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public void Create_IfTheSourceParameterIsWhitespace_ShouldThrowAnArgumentException()
		{
			this.FileTransformerFactory.Create(" ");
		}

		#endregion
	}
}