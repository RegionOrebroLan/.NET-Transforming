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
	public class FileTransformerFactoryTest
	{
		#region Fields

		private static FileTransformerFactory _fileTransformerFactory;

		#endregion

		#region Properties

		protected internal virtual FileTransformerFactory FileTransformerFactory => _fileTransformerFactory ??= new FileTransformerFactory(Mock.Of<IFileSystem>(), Mock.Of<ILoggerFactory>(), Mock.Of<IOptionsMonitor<TransformingOptions>>());

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